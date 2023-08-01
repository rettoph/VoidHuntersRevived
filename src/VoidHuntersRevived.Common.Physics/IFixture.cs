namespace VoidHuntersRevived.Common.Physics
{
    public interface IFixture
    {
        VhId Id { get; }

        FixVector2 Centeroid { get; }

        IBody Body { get; }
    }
}
