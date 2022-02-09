using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Interfaces;
using Guppy.Threading.Interfaces;
using Guppy.Threading.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.MessageProcessors
{
    /// <summary>
    /// Often times transient services should process specific messages directed towards them.
    /// The current bus system doesnt easily allow for targeting specific instances based on 
    /// message data. This class will implement a nice template for creating "consolidated"
    /// processors. That is, a scoped service to manage all messages of a specific type
    /// and automatically forward them to the given processor.
    /// 
    /// The only catch is that each processor is responnsible for registering itself to the 
    /// ConsolidationProcessor instance.
    /// 
    /// Take a look at <see cref="TractorBeamRequestProcessor"/> for a good example of how this might work.
    /// </summary>
    /// <typeparam name="TConsolidableMessage"></typeparam>
    /// <typeparam name="TTransientProcessor"></typeparam>
    public abstract class ConsolidationProcessor<TConsolidableMessage, TTransientProcessor> : Service, IDataProcessor<TConsolidableMessage>
        where TConsolidableMessage : ConsolidableMessage
        where TTransientProcessor : IService, IDataProcessor<TConsolidableMessage>
    {
        #region Private Fields
        private MessageBus _bus;
        private Dictionary<Guid, TTransientProcessor> _transientProcessors;
        private ILogger _logger;
        #endregion

        #region Public Properties
        public abstract Int32 MessageQueue { get; }
        #endregion

        #region Lifecycle Methods
        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            provider.Service(out _bus);
            provider.Service(out _logger);

            _transientProcessors = new Dictionary<Guid, TTransientProcessor>();

            _bus.ConfigureMessageTypes(this.MessageQueue, typeof(TConsolidableMessage));
            _bus.RegisterProcessor<TConsolidableMessage>(this);

            Debug.Assert(
                this.ServiceConfiguration.Lifetime == ServiceLifetime.Scoped,
                $"{this.GetType().GetPrettyName()} - {nameof(ServiceLifetime)} of {nameof(ServiceLifetime.Scoped)} expected. Service {this.ServiceConfiguration.Name} is set to {this.ServiceConfiguration.Lifetime}."
            );
        }

        protected override void PreUninitialize()
        {
            base.PreUninitialize();

            _bus.DeregisterProcessor<TConsolidableMessage>(this);
        }
        #endregion

        #region Helper Methods
        public void Enqueue(TConsolidableMessage message)
        {
            _bus.Enqueue(message);
        }

        public Boolean TryAdd(TTransientProcessor transientProcessor)
        {
            return _transientProcessors.TryAdd(transientProcessor.Id, transientProcessor);
        }

        public void Remove(TTransientProcessor transientProcessor)
        {
            _transientProcessors.Remove(transientProcessor.Id);
        }
        #endregion

        #region Message Processors
        bool IDataProcessor<TConsolidableMessage>.Process(TConsolidableMessage action)
        {
            if (_transientProcessors.TryGetValue(action.Id, out TTransientProcessor transientProcessor))
            {
                return transientProcessor.Process(action);
            }

            return false;
        }
        #endregion
    }
}
