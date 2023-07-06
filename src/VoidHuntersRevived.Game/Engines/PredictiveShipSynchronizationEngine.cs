using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Enums;
using VoidHuntersRevived.Game.Pieces.Components;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal class PredictiveShipSynchronizationEngine : BasicEngine, IPredictiveSynchronizationEngine
    {
        private readonly ISpace _predictiveSpace;
        private ISpace _lockstepSpace;

        public PredictiveShipSynchronizationEngine(ISpace space)
        {
            _predictiveSpace = space;
            _lockstepSpace = null!;
        }

        public void Initialize(ILockstepSimulation lockstep)
        {
            _lockstepSpace = lockstep.Scope.ServiceProvider.GetRequiredService<ISpace>();
        }

        public void Synchronize(Step step)
        {
            Fix64 damping = step.ElapsedTime;

            foreach (IBody predictiveBody in _predictiveSpace.AllBodies())
            {
                if(!_lockstepSpace.TryGetBody(predictiveBody.Id, out IBody? lockstepBody))
                {
                    continue;
                }

                predictiveBody.SetTransform(
                    position: FixVector2.Lerp(predictiveBody.Position, lockstepBody.Position, damping),
                    rotation: Fix64.Lerp(predictiveBody.Rotation, lockstepBody.Rotation, damping));

                predictiveBody.SetVelocity(
                    linear: FixVector2.Lerp(predictiveBody.LinearVelocity, lockstepBody.LinearVelocity, damping),
                    angular: Fix64.Lerp(predictiveBody.AngularVelocity, lockstepBody.AngularVelocity, damping));
            }
        }
    }
}
