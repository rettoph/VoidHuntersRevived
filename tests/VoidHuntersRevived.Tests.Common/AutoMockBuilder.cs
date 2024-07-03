using Autofac;
using Autofac.Extras.Moq;

namespace VoidHuntersRevived.Tests.Common
{
    public class AutoMockBuilder
    {
        private Action<ContainerBuilder>? _builders;

        public AutoMockBuilder()
        {
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
            return AutoMock.GetLoose(_builders);
        }
    }
}
