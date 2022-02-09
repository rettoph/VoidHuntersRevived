using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.MessageProcessors;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Components
{
    /// <summary>
    /// Helper component useful for integrating directly with a <see cref="ConsolidationProcessor"/> in order to bul process messages in order.
    /// </summary>
    public abstract class ConsolidableTransientProcessorComponent<TEntity, TConsolidableMessage, TConsolidationProcessor, TTransientProcessor> : Component<TEntity>, IDataProcessor<TConsolidableMessage>
        where TEntity : Entity
        where TConsolidableMessage : ConsolidableMessage
        where TConsolidationProcessor : ConsolidationProcessor<TConsolidableMessage, TTransientProcessor>
        where TTransientProcessor : ConsolidableTransientProcessorComponent<TEntity, TConsolidableMessage, TConsolidationProcessor, TTransientProcessor>
    {
        protected TConsolidationProcessor consolidationProcessor { get; private set; }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.consolidationProcessor = provider.GetService<TConsolidationProcessor>();
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.consolidationProcessor.TryAdd(this as TTransientProcessor);
        }

        protected override void PreUninitialize()
        {
            base.PreUninitialize();

            this.consolidationProcessor.Remove(this as TTransientProcessor);
        }

        public abstract Boolean Process(TConsolidableMessage request);
    }
}
