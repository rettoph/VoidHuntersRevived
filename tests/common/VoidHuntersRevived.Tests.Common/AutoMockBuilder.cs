using Autofac;
using Autofac.Extras.Moq;
using Guppy.Core.Common.Contexts;
using Guppy.Core.Extensions;
using Guppy.Tests.Common;

namespace VoidHuntersRevived.Tests.Common
{
    public class AutoMockBuilder
    {
        private Action<ContainerBuilder>? _builders;

        public AutoMockBuilder()
        {
            this.Register(x => x.RegisterCoreServices(
                context: Mocker<IGuppyContext>.Create().AsMock().Object));
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

        public AutoMock Build()
        {
            return AutoMock.GetLoose(builder => _builders?.Invoke(builder));
        }
    }
}
