using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Filters;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Simulations.Common.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterEngine<T>(this IGuppyScopeBuilder builder)
            where T : IEngine
        {
            builder.RegisterType<T>().AsImplementedInterfaces().InstancePerLifetimeScope();
            return builder;
        }

        public static IGuppyScopeBuilder RegisterStrategyFilter(this IGuppyScopeBuilder builder, Type serviceType, Type? strategyType)
        {
            if (strategyType is not null)
            {
                ThrowIf.Type.IsNotAssignableFrom<IStrategy>(strategyType);
            }

            return builder.RegisterFilter(new StateServiceFilter<Type?>(serviceType, StateKey<Type?>.Create<IStrategy>(), strategyType));
        }

        public static IGuppyScopeBuilder RegisterStrategyFilter<TService, TStrategy>(this IGuppyScopeBuilder builder)
            where TStrategy : IStrategy
        {
            return builder.RegisterFilter(new StateServiceFilter<Type?>(typeof(TService), StateKey<Type?>.Create<IStrategy>(), typeof(TStrategy)));
        }
    }
}