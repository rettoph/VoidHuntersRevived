using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    public class BodyLocationPredictiveSynchronizationSystem(
        ISpace space,
        ILogger<BodyLocationPredictiveSynchronizationSystem> logger
    ) : ISceneSystem,
        IPredictiveSynchronizationSystem
    {
        private readonly ISpace _predictiveSpace = space;
        private readonly ILogger<BodyLocationPredictiveSynchronizationSystem> _logger = logger;
        private ISpace _lockstepSpace = null!;

        public void Initialize(ILockstepStrategy lockstep)
        {
            this._lockstepSpace = lockstep.Resolve<ISpace>();
        }

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.SubmitChanges)]
        public void Synchronize(Step step)
        {
            Fix64 damping = step.ElapsedTime / (Fix64)0.25f;

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

                FixVector2 targetPosition = FixVector2.Lerp(predictiveBody.Position, lockstepBody.Position, damping);
                Fix64 targetRotation = Fix64.Lerp(predictiveBody.Rotation, lockstepBody.Rotation, damping);

                FixVector2 targetLinearVelocity = FixVector2.Lerp(predictiveBody.LinearVelocity, lockstepBody.LinearVelocity, damping);
                Fix64 targetAngularVelocity = Fix64.Lerp(predictiveBody.AngularVelocity, lockstepBody.AngularVelocity, damping);

                //this._logger.Verbose(
                //    "Synchronizing. PredictiveRotation = {PredictiveRotation}, TargetRotation = {TargetRotation}, LockstepRotation = {LockstepRotation}, Damping = {Damping}",
                //    predictiveBody.Rotation,
                //    targetRotation,
                //    lockstepBody.Rotation,
                //    damping);

                predictiveBody.SetTransform(
                    position: targetPosition,
                    rotation: targetRotation);

                predictiveBody.SetVelocity(
                    linear: targetLinearVelocity,
                    angular: targetAngularVelocity);
            }
        }
    }
}