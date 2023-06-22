namespace VoidHuntersRevived.Common.Physics
{
    public interface IFixture
    {
        VhId Id { get; }

        IBody Body { get; }
    }
}
