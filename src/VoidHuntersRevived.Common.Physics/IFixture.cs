namespace VoidHuntersRevived.Common.Physics
{
    public interface IFixture
    {
        Guid Guid { get; }

        IBody Body { get; }
    }
}
