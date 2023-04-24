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
                private static Dictionary<Type, Func<SimulationType, ParallelKey, IData, ISimulation, IInput>> _factories = new();
                private static MethodInfo _method = typeof(Factory).GetMethod(nameof(FactoryMethod), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new UnreachableException();

                public static IInput Create(SimulationType source, ParallelKey sender, IData data, ISimulation target)
                {
                    var type = data.GetType();
                    if (!_factories.TryGetValue(type, out var factory))
                    {
                        var method = _method.MakeGenericMethod(type);
                        factory = (Func<SimulationType, ParallelKey, IData, ISimulation, IInput>)(method.Invoke(null, Array.Empty<object>()) ?? throw new UnreachableException());

                        _factories.Add(type, factory);
                    }

                    return factory(source, sender, data, target);
                }

                private static Func<SimulationType, ParallelKey, IData, ISimulation, IInput> FactoryMethod<TData>()
                    where TData : IData
                {
                    IInput Factory(SimulationType source, ParallelKey sender, IData data, ISimulation target)
                    {
                        return new Simulation.Event<TData>(source, sender, (TData)data, target);
                    }

                    return Factory;
                }
            }
        }

        protected class Event<TData> : IMessage, IInput<TData>
            where TData : notnull, IData
        {
            private static readonly Type MessageType = typeof(IInput<TData>);

            public virtual Type Type => MessageType;

            public SimulationType Source { get; }

            public ParallelKey Sender { get; }

            public TData Data { get; }

            public ISimulation Simulation { get; }

            IData IInput.Data => this.Data;

            public Event(
                SimulationType source, 
                ParallelKey sender, 
                TData data, 
                ISimulation simulation)
            {
                this.Source = source;
                this.Sender = sender;
                this.Data = data;
                this.Simulation = simulation;
            }
        }
    }
}
