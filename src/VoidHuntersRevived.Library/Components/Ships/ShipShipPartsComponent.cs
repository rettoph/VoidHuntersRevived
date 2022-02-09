using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Components;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.MessageProcessors;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Components.Ships
{
    /// <summary>
    /// Simple helper component designed to track a collection of ShipPart's
    /// within the ship's chain.
    /// 
    /// This also extends <see cref="ConsolidableTransientProcessorComponent"/>, meaning it can 
    /// react to <see cref="ConsolidableMessage"/> requests. Useful for applying some effect
    /// to the stored parts within. Example: On Direction request update all internal thrusters.
    /// </summary>
    public abstract class ShipShipPartsComponent<TShipPart, TConsolidableMessage, TConsolidationProcessor, TTransientProcessor> : ConsolidableTransientProcessorComponent<Ship, TConsolidableMessage, TConsolidationProcessor, TTransientProcessor>
        where TShipPart : ShipPart
        where TConsolidableMessage : ConsolidableMessage
        where TConsolidationProcessor : ConsolidationProcessor<TConsolidableMessage, TTransientProcessor>
        where TTransientProcessor : ConsolidableTransientProcessorComponent<Ship, TConsolidableMessage, TConsolidationProcessor, TTransientProcessor>
    {
        #region Private Fields
        private List<TShipPart> _shipParts;
        #endregion

        #region Public Properties
        public IEnumerable<TShipPart> ShipParts => _shipParts;
        #endregion

        #region Events
        internal event OnEventDelegate<IEnumerable<TShipPart>> OnShipPartsUpdated;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _shipParts = new List<TShipPart>();

            this.Entity.Chain.Root.PostTreeClean += this.HandleChainRootPostTreeClean;
            this.UpdateShipParts(this.Entity.Chain.Root);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            _shipParts.Clear();

            this.Entity.Chain.Root.PostTreeClean -= this.HandleChainRootPostTreeClean;
        }
        #endregion

        #region Helper Methods
        protected virtual Boolean ShouldTrack(TShipPart shipPart)
        {
            return true;
        }

        private void UpdateShipParts(ShipPart root)
        {
            _shipParts.Clear();

            foreach (ShipPart shipPart in root.GetChildren())
            {
                if(shipPart is TShipPart casted && this.ShouldTrack(casted))
                {
                    _shipParts.Add(casted);
                }
            }

            this.OnShipPartsUpdated?.Invoke(_shipParts);
        }
        #endregion

        #region Event Handlers
        private void HandleChainRootPostTreeClean(ShipPart sender, ShipPart source, TreeComponent components)
        {
            this.UpdateShipParts(sender);
        }
        #endregion
    }
}
