using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal abstract class Input : IInput
    {
        public Guid Id { get; }

        public ParallelKey Sender { get; }

        public IData Data { get; }

        public ISimulation Simulation { get; }

        public Type Type { get; }

        protected Input(Guid id, ISimulation simulation, ParallelKey sender, IData data, Type type)
        {
            this.Id = id;
            this.Simulation = simulation;
            this.Sender = sender;
            this.Data = data;
            this.Type = type;
        }

        private static Dictionary<Type, Func<ISimulation, InputDto, IInput>> _factories = new();
        private static MethodInfo _method = typeof(Input).GetMethod(nameof(FactoryMethod), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new UnreachableException();

        public static IInput Create(ISimulation simulation, InputDto dto)
        {
            var type = dto.Data.GetType();
            if (!_factories.TryGetValue(type, out var factory))
            {
                var method = _method.MakeGenericMethod(type);
                factory = (Func<ISimulation, InputDto, IInput>)(method.Invoke(null, Array.Empty<object>()) ?? throw new UnreachableException());

                _factories.Add(type, factory);
            }

            return factory(simulation, dto);
        }

        private static Func<ISimulation, InputDto, IInput> FactoryMethod<TData>()
            where TData : IData
        {
            IInput Factory(ISimulation simulation, InputDto dto)
            {
                return new Input<TData>(dto.Id, simulation, dto.Sender, (TData)dto.Data);
            }

            return Factory;
        }
    }

    internal class Input<TData> : Input, IInput<TData>
        where TData : IData
    {
        public new TData Data { get; }

        internal Input(Guid id, ISimulation simulation, ParallelKey sender, TData data) : base(id, simulation, sender, data, typeof(IInput<TData>))
        {
            this.Data = data;
        }
    }
}
