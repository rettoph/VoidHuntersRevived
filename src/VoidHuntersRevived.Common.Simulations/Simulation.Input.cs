using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public partial class Simulation
    {
        public static class Input
        {
            public static class Factory
            {
                private static Dictionary<Type, Func<SimulationType, ParallelKey, IData, ISimulation, IEvent>> _factories = new();
                private static MethodInfo _method = typeof(Factory).GetMethod(nameof(FactoryMethod), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new UnreachableException();

                public static IEvent Create(SimulationType sender, ParallelKey user, IData data, ISimulation simulation)
                {
                    var type = data.GetType();
                    if (!_factories.TryGetValue(type, out var factory))
                    {
                        var method = _method.MakeGenericMethod(type);
                        factory = (Func<SimulationType, ParallelKey, IData, ISimulation, IEvent>)(method.Invoke(null, Array.Empty<object>()) ?? throw new UnreachableException());

                        _factories.Add(type, factory);
                    }

                    return factory(sender, user, data, simulation);
                }

                private static Func<SimulationType, ParallelKey, IData, ISimulation, IEvent> FactoryMethod<TData>()
                    where TData : IData
                {
                    IEvent Factory(SimulationType sender, ParallelKey user, IData data, ISimulation simulation)
                    {
                        return new Simulation.Input<TData>(sender, user, (TData)data, simulation);
                    }

                    return Factory;
                }
            }
        }

        protected class Input<TData> : Event<TData>, IInput<TData>
            where TData : notnull, IData
        {
            private static readonly Type MessageType = typeof(IInput<TData>);

            public ParallelKey PilotKey { get; }

            public override Type Type => MessageType;

            public Input(SimulationType sender, ParallelKey user, TData data, ISimulation simulation) : base(sender, data, simulation)
            {
                this.PilotKey = user;
            }
        }
    }
}
