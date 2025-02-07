using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    public class BodyLocationPredictiveSynchronizationSystem(ISpace space) : StrategySystem, IPredictiveSynchronizationSystem
    {
        private readonly ISpace _predictiveSpace = space;
        private ISpace _lockstepSpace = null!;

        public void Initialize(ILockstepStrategy lockstep)
        {
            this._lockstepSpace = lockstep.Resolve<ISpace>();
        }

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.SubmitChanges)]
        public void Synchronize(Step step)
        {
            Fix64 damping = step.ElapsedTime;

            foreach (IBody lockstepBody in this._lockstepSpace.AllBodies())
            {
                if (lockstepBody.Awake == false)
                {
                    continue;
                }

                if (!this._predictiveSpace.TryGetBody(lockstepBody.EntityLocalId, out IBody? predictiveBody))
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