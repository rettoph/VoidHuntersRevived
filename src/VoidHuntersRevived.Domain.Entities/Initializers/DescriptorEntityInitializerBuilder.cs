namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    internal sealed class DescriptorEntityInitializerBuilder : BaseEntityInitializerBuilder
    {
        public readonly Type DescriptorType;

        public DescriptorEntityInitializerBuilder(Type descriptorType)
        {
            this.DescriptorType = descriptorType;
        }
    }
}
