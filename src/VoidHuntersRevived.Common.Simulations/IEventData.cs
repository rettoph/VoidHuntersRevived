namespace VoidHuntersRevived.Common.Simulations
{
    public interface IEventData
    {
        bool IsPredictable { get; }
        bool IsLocalOnly => true;

        VhId CalculateHash(in VhId source);
    }
}
