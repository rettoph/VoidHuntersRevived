namespace VoidHuntersRevived.Common.Physics
{
    public interface IFixture
    {
        VhId VhId { get; }

        IBody Body { get; }
    }
}
