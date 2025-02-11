namespace VoidHuntersRevived.Domain.Simulations.Common.Enums
{
    /// <summary>
    /// Note: These values are opposite those of <see cref="EventSequenceGroupEnum"/>
    /// This is because we want to deinitialize in reverse order of initialization:
    /// 
    /// This means PreProcess initialize methods will be called first but PreProcess revert methods will
    /// be called last
    /// </summary>
    public enum RevertEventSequenceGroupEnum
    {
        PostProcess,
        Process,
        PreProcess
    }
}
