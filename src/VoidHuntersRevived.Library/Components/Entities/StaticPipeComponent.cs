using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Components.Entities
{
    /// <summary>
    /// Defines basic scaffolding for a basic component that will set a network entity's
    /// <see cref="INetworkEntity.Pipe"/> value on initialization.
    /// </summary>
    /// <typeparam name="TNetworkEntity"></typeparam>
    public abstract class StaticPipeComponent<TNetworkEntity> : RemoteHostComponent<TNetworkEntity>
        where TNetworkEntity : class, INetworkEntity
    {
        #region Lifeycycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Pipe = this.GetPipe(provider, provider.GetService<PrimaryScene>());

            this.Entity.OnPipeChanged += this.HandleStaticPipeEntityPipeChanged;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.OnPipeChanged -= this.HandleStaticPipeEntityPipeChanged;

            this.Entity.Pipe = default;
        }
        #endregion

        #region Helper Methods
        protected abstract IPipe GetPipe(GuppyServiceProvider provider, PrimaryScene scene);
        #endregion

        #region Event Handlers

        private void HandleStaticPipeEntityPipeChanged(INetworkEntity sender, IPipe old, IPipe value)
        {
            throw new AccessViolationException($"Invalid NetworkEntity.Pipe change detected. This should never happen once StaticPipeComponent has been registered to the entity.");
        }
        #endregion
    }
}
