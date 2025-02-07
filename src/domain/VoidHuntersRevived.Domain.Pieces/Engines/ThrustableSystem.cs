using System.Xml.Linq;
using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Systems;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Domain.Pieces.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Pieces.Systems
{
    public class ThrustableSystem(
        IEntityQueryService entityQueryService,
        ISpace space
    ) : ISceneSystem,
        IOnSpawnSystem<Thrustable>,
        IOnDespawnSystem<Thrustable>,
        IEventSystem<Tree_Clean>,
        IOnStepSystem
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ISpace _space = space;

        private static readonly Fix64 _buffer = (Fix64)0.01m;
        private static readonly Fix64 _bufferPi = Fix64.Pi - _buffer;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Thrustable> thrustable)
        {
            Node node = this._entityQueryService.QueryByGroupIndex<Node>(thrustable.GroupIndex);

            if (this._entityQueryService.Has<Helm>(node.TreeLocalId.Value.groupID) == false)
            {
                return;
            }

            ref var filter = ref this._entityQueryService.GetCompositeFilter<Body, Fixture, Thrustable>(node.TreeLocalId);
            filter.Add(thrustable.LocalId, thrustable.Index);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Thrustable> thrustable)
        {
            Node node = this._entityQueryService.QueryByGroupIndex<Node>(thrustable.GroupIndex);

            if (this._entityQueryService.Has<Helm>(node.TreeLocalId.Value.groupID) == false)
            {
                return;
            }

            ref var filter = ref this._entityQueryService.GetCompositeFilter<Body, Fixture, Thrustable>(node.TreeLocalId);
            filter.Remove(thrustable.LocalId);
        }

        public void Process(VhId eventId, Tree_Clean data)
        {
            if (!this._entityQueryService.TryGetLocalId(data.TreeGlobalId, out EntityLocalId treeLocalId))
            {
                return;
            }
            if (!this._entityQueryService.Has<Helm>(treeLocalId.Group))
            {
                return;
            }
            if (this._entityQueryService.TryQueryByLocalId<Enabled>(treeLocalId, out Enabled enabled) == false || enabled == false)
            {
                return;
            }

            IBody treeBody = this._space.GetBody(treeLocalId);
            ref var filter = ref this._entityQueryService.GetCompositeFilter<Body, Fixture, Thrustable>(treeLocalId);

            foreach (var (thrustableIndices, group) in filter)
            {
                var (thrustables, fixtures, _) = this._entityQueryService.QueryEntities<Thrustable, Fixture>(group);

                for (int i = 0; i < thrustableIndices.count; i++)
                {
                    uint index = thrustableIndices[i];
                    CleanThrustable(treeBody, ref thrustables[index], ref fixtures[index]);
                }
            }
        }

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.ProcessInput)]
        public void OnStep(Step step)
        {
            foreach (var ((localIds, enableds, helms, count), _) in this._entityQueryService.QueryEntities<EntityLocalId, Enabled, Helm>())
            {
                for (int i = 0; i < count; i++)
                {
                    EntityLocalId helmLocalId = localIds[i];
                    Helm helm = helms[i];
                    Enabled enabled = enableds[i];

                    if (enabled == false)
                    {
                        continue;
                    }

                    IBody body = this._space.GetBody(helmLocalId);
                    ref var filter = ref this._entityQueryService.GetCompositeFilter<Body, Fixture, Thrustable>(helmLocalId);

                    this.TryApplyImpulse(body, helm.Direction, ref filter);
                }
            }
        }

        private void TryApplyImpulse(IBody body, DirectionEnum direction, ref EntityFilterCollection filter)
        {
            foreach (var (indices, group) in filter)
            {
                var (thrustables, fixtures, _) = this._entityQueryService.QueryEntities<Thrustable, Fixture>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    ref Thrustable thrustable = ref thrustables[index];

                    if ((thrustable.Direction & direction) == 0)
                    {
                        continue;
                    }

                    ref Fixture fixture = ref fixtures[index];

                    body.ApplyForce(
                        force: FixPolar.Rotate(thrustable.MaxImpulse, fixture.WorldRotation).ToVector2(),
                        point: FixVector2.Transform(thrustable.ImpulsePoint, fixture.WorldTransform));
                }
            }
        }

        private static void CleanThrustable(IBody treeBody, ref Thrustable thrustable, ref Fixture fixture)
        {
            thrustable.Direction = DirectionEnum.None;

            // The chain's center of mass
            var com = treeBody.LocalCenter;
            // The point acceleration is applied
            var ip = FixVector2.Transform(thrustable.ImpulsePoint, fixture.LocalTransform);
            // The impulse to be applied...
            var i = FixPolar.Rotate(thrustable.MaxImpulse, fixture.LocalRotation).ToVector2();
            // The point acceleration is targeting
            var it = ip + i;

            // The angle between the com and the acceleration point
            var ipr = Fix64.WrapAngle(Fix64.Atan2(ip.Y - com.Y, ip.X - com.X));
            // The angle between the com and the acceleration target
            _ = Fix64.WrapAngle(Fix64.Atan2(it.Y - com.Y, it.X - com.X));
            // The angle between the acceleration point and the acceleration target
            var ipitr = Fix64.WrapAngle(Fix64.Atan2(it.Y - ip.Y, it.X - ip.X));
            // The relative acceleration target rotation between the acceleration point and center of mass
            var ript = Fix64.WrapAngle(ipitr - ipr);

            // Define some lower and upper bounds...
            var ipitr_lower = ipitr - _buffer;
            var ipitr_upper = ipitr + _buffer;

            // Check if the thruster moves the chain forward...
            if ((ipitr_upper < Fix64.PiOver2 && ipitr_lower > -Fix64.PiOver2))
            {
                thrustable.Direction |= DirectionEnum.Forward;
            }

            // Check if the thruster turns the chain right...
            if (ript > _buffer && ript < _bufferPi)
            {
                thrustable.Direction |= DirectionEnum.TurnRight;
            }

            // Check if the thruster moves the chain backward...
            if (ipitr_lower > Fix64.PiOver2 || ipitr_upper < -Fix64.PiOver2)
            {
                thrustable.Direction |= DirectionEnum.Backward;
            }

            // Check if the thruster turns the chain left...
            if (ript < -_buffer && ript > -_bufferPi)
            {
                thrustable.Direction |= DirectionEnum.TurnLeft;
            }

            // Check if the thruster moves the chain right...
            if (ipitr_lower < -_buffer && ript > -_bufferPi)
            {
                thrustable.Direction |= DirectionEnum.Right;
            }

            // Check if the thruster moves the chain left...
            if (ipitr_lower > _buffer && ipitr_upper < _bufferPi)
            {
                thrustable.Direction |= DirectionEnum.Left;
            }
        }
    }
}