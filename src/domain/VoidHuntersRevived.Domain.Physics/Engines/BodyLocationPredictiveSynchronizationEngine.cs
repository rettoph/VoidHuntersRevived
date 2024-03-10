using Autofac;
using Guppy.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    internal class BodyLocationPredictiveSynchronizationEngine : BasicEngine, IPredictiveSynchronizationEngine
    {
        private readonly ISpace _predictiveSpace;
        private ISpace _lockstepSpace;

        public BodyLocationPredictiveSynchronizationEngine(ISpace space)
        {
            _predictiveSpace = space;
            _lockstepSpace = null!;
        }

        public void Initialize(ILockstepSimulation lockstep)
        {
            _lockstepSpace = lockstep.Scope.Resolve<ISpace>();
        }

        public void Synchronize(Step step)
        {
            Fix64 damping = step.ElapsedTime;

            foreach (IBody lockstepBody in _lockstepSpace.AllBodies())
            {
                if (lockstepBody.Awake == false)
                {
                    continue;
                }

                if (!_predictiveSpace.TryGetBody(lockstepBody.Id, out IBody? predictiveBody))
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
