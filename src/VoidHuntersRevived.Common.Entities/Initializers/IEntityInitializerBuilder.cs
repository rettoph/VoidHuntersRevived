namespace VoidHuntersRevived.Common.Entities.Initializers
{
    public interface IEntityInitializerBuilder
    {
        IEntityInitializerBuilder InitializeInstance(InstanceEntityInitializerDelegate initializer);
        IEntityInitializerBuilder DisposeInstance(DisposeEntityInitializerDelegate disposer);

        IEntityInitializerBuilder InitializeStatic(StaticEntityInitializerDelegate initializer);
        IEntityInitializerBuilder DisposeStatic(DisposeEntityInitializerDelegate disposer);
    }
}
