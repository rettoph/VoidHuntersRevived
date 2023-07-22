using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Factories;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Game.Services;
using Svelto.DataStructures;
using VoidHuntersRevived.Game.Enums;
using VoidHuntersRevived.Domain.Common.Components;
using Serilog;
using Svelto.ECS.Native;
using System.Xml.Linq;
using VoidHuntersRevived.Common.Pieces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.FixedPoint.Extensions;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterUpdateEngine : BasicEngine,
        IStepEngine<Step>
    {
        private readonly IEntityIdService _entities;
        private readonly ISpace _space;
        private readonly IFilterService _filters;
        private readonly ILogger _logger;
        private readonly TractorBeamEmitterService _tractorBeamEmitterService;

        public string name { get; } = nameof(TractorBeamEmitterUpdateEngine);

        public TractorBeamEmitterUpdateEngine(
            IEntityIdService entities, 
            ISpace space,
            IFilterService filters,
            ILogger logger,
            TractorBeamEmitterService tractorBeamEmitterService)
        {
            _entities = entities;
            _space = space;
            _filters = filters;
            _logger = logger;
            _tractorBeamEmitterService = tractorBeamEmitterService;
        }

        public void Step(in Step _param)
        {
            var groups = this.entitiesDB.FindGroups<Tactical, TractorBeamEmitter>();
            foreach (var ((vhids, tacticals, tractorBeamEmitters, count), _) in this.entitiesDB.QueryEntities<EntityVhId, Tactical, TractorBeamEmitter>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    this.UpdateTractorBeamEmitterTarget(in vhids[i].Value, ref tacticals[i], ref tractorBeamEmitters[i]);
                }
            }
        }

        private void UpdateTractorBeamEmitterTarget(in VhId shipId, ref Tactical tactical, ref TractorBeamEmitter tractorBeamEmitter)
        {
            if(tractorBeamEmitter.Active == false)
            {
                return;
            }

            IBody targetBody = _space.GetBody(in tractorBeamEmitter.TargetVhId);

            EntityId targetId = _entities.GetId(tractorBeamEmitter.TargetVhId);
            ref Tree target = ref this.entitiesDB.QueryEntity<Tree>(targetId.EGID);

            EntityId targetHeadId = _entities.GetId(target.HeadVhId);
            Joint targetHeadChildNode = this.entitiesDB.QueryEntity<Joints>(targetHeadId.EGID).Child;

            if (this.TryGetClosestOpenJointOnShipToTactical(shipId, ref tactical, out var openNodeJoint))
            {
                FixMatrix potentialTransformation = targetHeadChildNode.Location.Transformation * openNodeJoint.Transformation;
                FixVector2 potentialPosition = FixVector2.Transform(FixVector2.Zero, potentialTransformation);

                targetBody.SetTransform(potentialPosition, Fix64.Pi - openNodeJoint.Transformation.Radians());

                return;
            }

            FixVector2 targetHeadChildNodePosition = FixVector2.Transform(targetHeadChildNode.Location.Position, FixMatrix.CreateRotationZ(targetBody.Rotation));
            targetBody.SetTransform(tactical.Value - targetHeadChildNodePosition, targetBody.Rotation);
        }

        private bool TryGetClosestOpenJointOnShipToTactical(VhId shipId, ref Tactical tactical, out NodeJoint nodeJoint)
        {
            // Since ships are Trees the ShipId will be the filterId seen in NodeEngine
            ref var filter = ref _filters.GetFilter<Node>(shipId);
            Fix64 closestOpenJointOnShipToTacticalDistance = Fix64.One;
            nodeJoint = default!;
            bool result = false;

            foreach (var (indeces, group) in filter)
            {
                if (!this.entitiesDB.HasAny<Joints>(group))
                {
                    continue;
                }

                var (nodes, jointses, _) = entitiesDB.QueryEntities<Node, Joints>(group);

                for (int i = 0; i < indeces.count; i++)
                {
                    uint index = indeces[i];
                    if(
                        this.TryGetClosestOpenJointOnNodeToTactical(ref tactical, ref nodes[index], ref jointses[index], out Fix64 closestOpenJointOnNodeToTacticalDistance, out var closestOpenNodeJointOnNodeToTactical)
                        && closestOpenJointOnNodeToTacticalDistance < closestOpenJointOnShipToTacticalDistance)
                    {
                        closestOpenJointOnShipToTacticalDistance = closestOpenJointOnNodeToTacticalDistance;
                        nodeJoint = new NodeJoint(ref closestOpenNodeJointOnNodeToTactical.Node, ref closestOpenNodeJointOnNodeToTactical.Joint);
                        result = true;
                    }
                }
            }

            return result;
        }

        private bool TryGetClosestOpenJointOnNodeToTactical(
            ref Tactical tactical, 
            ref Node node, 
            ref Joints joints, 
            out Fix64 closestOpenJointToTacticalDistance,
            out NodeJoint closestOpenNodeJointOnNodeToTactical)
        {
            closestOpenJointToTacticalDistance = Fix64.One;
            closestOpenNodeJointOnNodeToTactical = default!;
            bool result = false;

            for (int j = 0; j < joints.Parents.count; j++)
            {
                FixMatrix jointWorldTransformation = joints.Parents[j].Location.Transformation * node.Transformation;
                FixVector2 jointWorldPosition = FixVector2.Transform(FixVector2.Zero, jointWorldTransformation);

                FixVector2.Distance(ref jointWorldPosition, ref tactical.Value, out Fix64 jointDistanceFromTactical);

                if (jointDistanceFromTactical > closestOpenJointToTacticalDistance)
                {
                    continue;
                }

                closestOpenJointToTacticalDistance = jointDistanceFromTactical;
                closestOpenNodeJointOnNodeToTactical = new NodeJoint(ref node, ref joints.Parents[j]);
                result = true;
            }

            return result;
        }
    }
}
