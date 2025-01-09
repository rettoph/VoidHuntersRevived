namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public enum OnStepSequenceGroupEnum
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