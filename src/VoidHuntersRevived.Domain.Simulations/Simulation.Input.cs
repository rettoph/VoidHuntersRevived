using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    public partial class Simulation
    {
        public static class Input
        {
            public static class Factory
            {
                private static Dictionary<Type, Func<SimulationType, int, IData, ISimulation, IEvent>> _factories = new();
                private static MethodInfo _method = typeof(Factory).GetMethod(nameof(FactoryMethod), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new UnreachableException();

                public static bool TryCreate(SimulationType sender, ParallelKey userKey, IData data, ISimulation simulation, [MaybeNullWhen(false)] out IEvent instance)
                {
                    if(!simulation.TryGetEntityId(userKey, out int userId))
                    {
                        instance = null;
                        return false;
                    }

                    var type = data.GetType();
                    if (!_factories.TryGetValue(type, out var factory))
                    {
                        var method = _method.MakeGenericMethod(type);
                        factory = (Func<SimulationType, int, IData, ISimulation, IEvent>)(method.Invoke(null, Array.Empty<object>()) ?? throw new UnreachableException());

                        _factories.Add(type, factory);
                    }

                    instance = factory(sender, userId, data, simulation);
                    return true;
                }

                private static Func<SimulationType, int, IData, ISimulation, IEvent> FactoryMethod<TData>()
                    where TData : IData
                {
                    IEvent Factory(SimulationType sender, int userId, IData data, ISimulation simulation)
                    {
                        return new Simulation.Input<TData>(sender, userId, (TData)data, simulation);
                    }

                    return Factory;
                }
            }
        }

        protected class Input<TData> : Event<TData>, IInput<TData>
            where TData : notnull, IData
        {
            private static readonly Type MessageType = typeof(IInput<TData>);

            public int UserId { get; }

            public override Type Type => MessageType;

            public Input(SimulationType sender, int userId, TData data, ISimulation simulation) : base(sender, data, simulation)
            {
                this.UserId = userId;
            }
        }
    }
}
