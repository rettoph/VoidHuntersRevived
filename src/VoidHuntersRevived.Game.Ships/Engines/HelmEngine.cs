using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Enums;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class HelmEngine : BasicEngine,
        IEventEngine<Helm_SetDirection>, IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public HelmEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
        }

        public string name { get; } = nameof(HelmEngine);

        public void Process(VhId vhid, Helm_SetDirection data)
        {
            EntityId id =_entities.GetId(data.ShipVhId);
            ref Helm helm = ref _entities.QueryById<Helm>(id);

            if (data.Value)
            {
                helm.Direction |= data.Which;
            }
            else
            {
                helm.Direction &= ~data.Which;
            }
        }

        public void Step(in Step _param)
        {
            foreach (var ((helms, _, entityIds, count), groupId) in _entities.QueryEntities<Helm, Location>())
            {
                for (int i = 0; i < count; i++)
                {
                    EntityId helmId = _entities.GetId(entityIds[i], groupId);
                    IBody body = _space.GetBody(helmId.VhId);

                    FixVector2 impulse = FixVector2.Zero;
                    Helm helm = helms[i];

                    if(helm.Direction.HasFlag(Direction.Forward))
                    {
                        impulse -= FixVector2.UnitY;
                    }
                    if (helm.Direction.HasFlag(Direction.Backward))
                    {
                        impulse += FixVector2.UnitY;
                    }
                    if (helm.Direction.HasFlag(Direction.TurnLeft))
                    {
                        impulse -= FixVector2.UnitX;
                    }
                    if (helm.Direction.HasFlag(Direction.TurnRight))
                    {
                        impulse += FixVector2.UnitX;
                    }

                    impulse *= _param.ElapsedTime;

                    body.ApplyLinearImpulse(impulse);
                }
            }
        }
    }
}
