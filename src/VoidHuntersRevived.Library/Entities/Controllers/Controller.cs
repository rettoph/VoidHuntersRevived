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
        private NetworkAuthorization _authorization;
        
        #endregion

        #region Protected Attributes
        protected IEnumerable<Chain> chains => _chains;
        protected ThreadSynchronizer synchronizer { get; private set; }
        #endregion

        #region Public Attributes
        /// <summary>
        /// Used to determin how a ShipPart should behave when
        /// container within the current controller.
        /// </summary>
        public NetworkAuthorization Authorization
        {
            get => _authorization;
            protected set => this.OnAuthorizationChanged.InvokeIfChanged(value != _authorization, this, ref _authorization, value);
        }
        #endregion

        #region Events
        public OnChangedEventDelegate<Controller, NetworkAuthorization> OnAuthorizationChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _chains = new HashSet<Chain>();

            // this.synchronizer = provider.GetService<ThreadSynchronizer>("synchronizer:controller");
            this.synchronizer = new ThreadSynchronizer();

            this.Authorization = provider.GetService<Settings>().Get<NetworkAuthorization>();
        }

        protected override void Release()
        {
            base.Release();

            _chains.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.synchronizer.Update(gameTime);
        }
        #endregion

        #region Helper Methods
        protected void TryAdd(Chain chain)
        {
            this.synchronizer.Do(gt =>
            {
                if (this.CanAdd(chain))
                    this.Add(chain);
            });
        }

        protected virtual Boolean CanAdd(Chain chain)
            => chain != default(Chain) && !_chains.Contains(chain);

        protected virtual void Add(Chain chain)
        {
            chain.Controller?.TryRemove(chain);
            _chains.Add(chain);
            chain.Controller = this;
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
            this.synchronizer.Do(gt => _chains.Remove(chain));

            chain.Controller = null;
        }
        #endregion
    }
}
