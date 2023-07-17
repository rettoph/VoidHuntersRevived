namespace VoidHuntersRevived.Common.Entities
{
    public interface IEventData
    {
        VhId CalculateHash(in VhId source);
    }
}
