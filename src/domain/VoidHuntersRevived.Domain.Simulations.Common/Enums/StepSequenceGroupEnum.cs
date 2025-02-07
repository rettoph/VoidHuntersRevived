namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public enum StepSequenceGroupEnum
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