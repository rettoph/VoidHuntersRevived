namespace VoidHuntersRevived.Common.ECS.Systems
{
    public abstract class BasicSystem : ISystem
    {
        public IWorld World { get; private set; } = null!;

        public virtual void Initialize(IWorld world)
        {
            this.World = world;
        }

        public virtual void Dispose()
        {
        }
    }
}
