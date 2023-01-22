using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    public partial class Simulation
    {
        public static class Event 
        {
            public static class Factory
            {
                private static Dictionary<Type, Func<SimulationType, IData, ISimulation, IEvent>> _factories = new();
                private static MethodInfo _method = typeof(Factory).GetMethod(nameof(FactoryMethod), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new UnreachableException();

                public static IEvent Create(SimulationType sender, IData data, ISimulation simulation)
                {
                    var type = data.GetType();
                    if (!_factories.TryGetValue(type, out var factory))
                    {
                        var method = _method.MakeGenericMethod(type);
                        factory = (Func<SimulationType, IData, ISimulation, IEvent>)(method.Invoke(null, Array.Empty<object>()) ?? throw new UnreachableException());

                        _factories.Add(type, factory);
                    }

                    return factory(sender, data, simulation);
                }

                private static Func<SimulationType, IData, ISimulation, IEvent> FactoryMethod<TData>()
                    where TData : IData
                {
                    IEvent Factory(SimulationType sender, IData data, ISimulation simulation)
                    {
                        return new Simulation.Event<TData>(sender, (TData)data, simulation);
                    }

                    return Factory;
                }
            }
        }

        protected class Event<TData> : IMessage, IEvent<TData>
            where TData : notnull, IData
        {
            private static readonly Type MessageType = typeof(IEvent<TData>);

            public virtual Type Type => MessageType;

            public SimulationType Sender { get; }

            public TData Data { get; }

            public ISimulation Simulation { get; }

            IData IEvent.Data => this.Data;

            public Event(SimulationType sender, TData data, ISimulation simulation)
            {
                this.Sender = sender;
                this.Data = data;
                this.Simulation = simulation;
            }
        }
    }
}
