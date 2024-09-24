namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public enum StepEngineSequenceGroup
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
