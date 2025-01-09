namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public enum OnStepSequenceGroup
    {
        Begin,

        SubmitChanges,

        StepSpace,

        SyncronizeEntities,

        ProcessInput,

        PublishEvents,

        End,
    }
}