using Guppy;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using System.Linq;
using VoidHuntersRevived.Library.Extensions.System.Collections;
using Microsoft.Xna.Framework;
using Guppy.Events.Delegates;
using Guppy.Utilities;

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
        protected Synchronizer synchronizer { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _chains = new HashSet<Chain>();

            this.synchronizer = provider.GetService<Synchronizer>();
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
            => chain != default(Chain) && !_chains.Contains(chain);

        protected virtual void Add(Chain chain)
        {
            chain.Controller?.TryRemove(chain);
            this.synchronizer.Enqueue(gt => _chains.Add(chain));
            chain.Controller = this;

            chain.OnReleased += this.HandleChainReleased;
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
            this.synchronizer.Enqueue(gt => _chains.Remove(chain));

            chain.Controller = null;

            chain.OnReleased -= this.HandleChainReleased;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Auto remove any chain that gets released from
        /// the stack.
        /// </summary>
        /// <param name="sender"></param>
        private void HandleChainReleased(IService sender)
            => this.Remove(sender as Chain);
        #endregion
    }
}
