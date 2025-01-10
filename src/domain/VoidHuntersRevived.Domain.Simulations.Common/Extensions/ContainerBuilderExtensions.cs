using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Filters;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Simulations.Common.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterEngine<T>(this ContainerBuilder builder)
            where T : IEngine
        {
            builder.RegisterType<T>().AsImplementedInterfaces().InstancePerLifetimeScope();
            return builder;
        }

        public static ContainerBuilder RegisterStrategyFilter(this ContainerBuilder builder, Type serviceType, Type? strategyType)
        {
            if (strategyType is not null)
            {
                ThrowIf.Type.IsNotAssignableFrom<IStrategy>(strategyType);
            }

            return builder.RegisterFilter(new StateServiceFilter<Type?>(serviceType, StateKey<Type?>.Create<IStrategy>(), strategyType));
        }

        public static ContainerBuilder RegisterStrategyFilter<TService, TStrategy>(this ContainerBuilder builder)
            where TStrategy : IStrategy => builder.RegisterFilter(new StateServiceFilter<Type?>(typeof(TService), StateKey<Type?>.Create<IStrategy>(), typeof(TStrategy)));
    }
}