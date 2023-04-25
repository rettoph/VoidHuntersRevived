using Guppy.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed class InputService : IInputService
    {
        private interface InputPublisher
        {
            void Publish(Input input, ISimulation simulation);
        }
        private class InputPublisher<T> : InputPublisher
            where T : Input
        {
            private readonly IInputSubscriber<T>[] _subscribers;

            public InputPublisher(IEnumerable<object> subscribers)
            {
                _subscribers = subscribers.OfType<IInputSubscriber<T>>().ToArray();
            }

            public void Publish(Input input, ISimulation simulation)
            {
                T casted = Unsafe.As<T>(input);

                foreach(IInputSubscriber<T> subscriber in _subscribers)
                {
                    subscriber.Process(casted, simulation);
                }
            }
        }

        private readonly Dictionary<Type, InputPublisher> _subscribers;
        private readonly IServiceProvider _provider;

        public InputService(IServiceProvider provider)
        {
            _provider = provider;
            _subscribers = new Dictionary<Type, InputPublisher>();
        }

        public void Publish(Input input, ISimulation simulation)
        {
            this.GetInputPublisher(input.GetType()).Publish(input, simulation);
        }

        public void Rollback(Input input, ISimulation simulation)
        {
            throw new NotImplementedException();
        }

        private InputPublisher GetInputPublisher(Type inputType)
        {
            ref InputPublisher? publisher = ref CollectionsMarshal.GetValueRefOrAddDefault(_subscribers, inputType, out bool exists);

            if (!exists)
            {
                Type publisherType = typeof(InputPublisher<>).MakeGenericType(inputType);
                Type subscriberType = typeof(IInputSubscriber<>).MakeGenericType(inputType);
                Type sortedType = typeof(ISorted<>).MakeGenericType(subscriberType);
                IEnumerable<object> subscribers = (IEnumerable<object>)_provider.GetService(sortedType)!;
                publisher = (InputPublisher)Activator.CreateInstance(publisherType, new[] { subscribers })!;
            }

            return publisher!;
        }
    }
}
