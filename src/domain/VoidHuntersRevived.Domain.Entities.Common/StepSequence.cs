namespace VoidHuntersRevived.Common.Entities
{
    public enum StepSequence
    {
        PreEntitySubmission = 0,
        EntitySubmission = 1,
        PostEntitySubmission = 2,

        PreResourceManagerUpdate = 3,
        ResourceManagerUpdate = 4,
        PostResourceManagerUpdate = 5,

        PreStep = 6,
        Step = 7,
        PostStep = 8,

        Cleanup = 9
    }
}
