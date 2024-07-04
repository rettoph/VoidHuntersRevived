using Autofac;
using Autofac.Extras.Moq;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Contexts;
using Guppy.Core.Extensions;
using Guppy.Tests.Common;
using System.Reflection;

namespace VoidHuntersRevived.Tests.Common
{
    public class AutoMockBuilder
    {
        private Action<ContainerBuilder>? _builders;

        public AutoMockBuilder()
        {
            this.Register(x => x.RegisterCoreServices(
                context: MockBuilder<IGuppyContext>.Create().Build().Object));
        }

        public static AutoMockBuilder Create()
        {
            return new AutoMockBuilder();
        }

        public AutoMockBuilder Register(Action<ContainerBuilder> builder)
        {
            _builders += builder;

            return this;
        }

        public AutoMockBuilder ConfigureAllGuppyConfigurationAttributesInAssembly(Assembly assembly)
        {
            return this.Register(builder =>
            {
                using (AutoMock boot = AutoMock.GetLoose())
                {
                    GuppyConfigurationAttribute.TryConfigureAllInAssembly(assembly, boot.Container, builder);
                }
            });
        }

        public AutoMock Build()
        {
            return AutoMock.GetLoose(builder => _builders?.Invoke(builder));
        }
    }
}
