using Guppy;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using Guppy.Enums;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// Controllers are entities that will 
    /// update self contained ShipParts as
    /// needed.
    /// </summary>
    public class Controller : Entity
    {
        #region Private Fields
        private HashSet<Chain> _chains;
        #endregion

        #region Protected Attributes
        protected IEnumerable<Chain> chains => _chains;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _chains = new HashSet<Chain>();
        }

        protected override void Release()
        {
            base.Release();

            _chains.Clear();
        }
        #endregion

        #region Helper Methods
        protected void TryAdd(Chain chain)
        {
            if (this.CanAdd(chain))
                this.Add(chain);
        }

        protected virtual Boolean CanAdd(Chain chain)
            => chain != default(Chain) && chain.Status != ServiceStatus.NotReady && !_chains.Contains(chain);

        protected virtual void Add(Chain chain)
        {
            chain.Controller?.TryRemove(chain);
            _chains.Add(chain);
            chain.Controller = this;

            chain.OnStatus[ServiceStatus.Releasing] += this.HandleChainReleasing;
        }

        protected virtual Boolean CanRemove(Chain chain)
            => _chains.Contains(chain);

        protected internal virtual void TryRemove(Chain chain)
        {
            if (this.CanRemove(chain))
                this.Remove(chain);
        }

        protected virtual void Remove(Chain chain)
        {
            _chains.Remove(chain);
            chain.Controller = null;

            chain.OnStatus[ServiceStatus.Releasing] -= this.HandleChainReleasing;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Auto remove any chain that gets released from
        /// the stack.
        /// </summary>
        /// <param name="sender"></param>
        private void HandleChainReleasing(IService sender)
            => this.Remove(sender as Chain);
        #endregion
    }
}
