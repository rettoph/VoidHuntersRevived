﻿using Guppy.Attributes;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Enums;
using VoidHuntersRevived.Common.Pieces.Events;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal class ThrustableEngine : BasicEngine,
        IReactOnAddEx<Thrustable>,
        IEventEngine<Tree_Clean>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        private static readonly Fix64 Buffer = (Fix64)0.01m;
        private static readonly Fix64 BufferPi = Fix64.Pi - Buffer;

        public ThrustableEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Thrustable> entities, ExclusiveGroupStruct groupID)
        {
            var (_, ids, _) = entities;
            var (nodes, _) = _entities.QueryEntities<Node>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                Node node = nodes[index];

                if (_entities.HasAny<Helm>(node.TreeId.EGID.groupID) == false)
                {
                    continue;
                }

                ref var filter = ref _entities.GetFilter<Thrustable>(node.TreeId, Helm.ThrustableFilterContextId);
                filter.Add(ids[index], groupID, index);
            }
        }

        public void Process(VhId eventId, Tree_Clean data)
        {
            if(!_entities.TryGetId(data.TreeId, out EntityId treeId))
            {
                return;
            }
            if(!_entities.HasAny<Helm>(treeId.EGID.groupID))
            {
                return;
            }

            IBody treeBody = _space.GetBody(treeId);
            ref var filter = ref _entities.GetFilter<Thrustable>(treeId, Helm.ThrustableFilterContextId);

            foreach (var (thrustableIndices, group) in filter)
            {
                var (thrustables, nodes, _) = _entities.QueryEntities<Thrustable, Node>(group);

                for (int i = 0; i < thrustableIndices.count; i++)
                {
                    uint index = thrustableIndices[i];
                    this.CleanThrustable(treeBody, ref thrustables[index], ref nodes[index]);
                }
            }
        }

        private void CleanThrustable(IBody treeBody, ref Thrustable thrustable, ref Node node)
        {
            thrustable.Direction = Direction.None;

            // The chain's center of mass
            var com = treeBody.LocalCenter;
            // The point acceleration is applied
            var ip = FixVector2.Transform(thrustable.ImpulsePoint, node.LocalTransformation);
            // The impulse to be applied...
            var i = FixVector2.Transform(thrustable.MaxImpulse, node.LocalTransformation.ToRotationMatrix());
            // The point acceleration is targeting
            var it = ip + i;

            // The angle between the com and the acceleration point
            var ipr = Fix64.WrapAngle(Fix64.Atan2(ip.Y - com.Y, ip.X - com.X));
            // The angle between the com and the acceleration target
            var itr = Fix64.WrapAngle(Fix64.Atan2(it.Y - com.Y, it.X - com.X));
            // The angle between the acceleration point and the acceleration target
            var ipitr = Fix64.WrapAngle(Fix64.Atan2(it.Y - ip.Y, it.X - ip.X));
            // The relative acceleration target rotation between the acceleration point and center of mass
            var ript = Fix64.WrapAngle(ipitr - ipr);

            // Define some lower and upper bounds...
            var ipitr_lower = ipitr - Buffer;
            var ipitr_upper = ipitr + Buffer;

            // Check if the thruster moves the chain forward...
            if ((ipitr_upper < Fix64.PiOver2 && ipitr_lower > -Fix64.PiOver2))
                thrustable.Direction |= Direction.Forward;

            // Check if the thruster turns the chain right...
            if (ript > Buffer && ript < BufferPi)
                thrustable.Direction |= Direction.TurnRight;

            // Check if the thruster moves the chain backward...
            if (ipitr_lower > Fix64.PiOver2 || ipitr_upper < -Fix64.PiOver2)
                thrustable.Direction |= Direction.Backward;

            // Check if the thruster turns the chain left...
            if (ript < -Buffer && ript > -BufferPi)
                thrustable.Direction |= Direction.TurnLeft;

            // Check if the thruster moves the chain right...
            if (ipitr_lower < -Buffer && ript > -BufferPi)
                thrustable.Direction |= Direction.Right;

            // Check if the thruster moves the chain left...
            if (ipitr_lower > Buffer && ipitr_upper < BufferPi)
                thrustable.Direction |= Direction.Left;
        }
    }
}
