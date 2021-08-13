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
        #region Private Fields
        private GuppyServiceProvider _provider;
        #endregion

        #region Lifeycycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            _provider = provider;

            this.Entity.OnStatus[ServiceStatus.Initializing] += this.HandleStaticPipeEntityInitializing;
        }


        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.Entity.OnStatus[ServiceStatus.Initializing] -= this.HandleStaticPipeEntityInitializing;
            this.Entity.OnPipeChanged -= this.HandleStaticPipeEntityPipeChanged;

            _provider = default;
        }
        #endregion

        #region Helper Methods
        protected abstract IPipe GetPipe(GuppyServiceProvider provider, PrimaryScene scene);
        #endregion

        #region Event Handlers
        private void HandleStaticPipeEntityInitializing(IService sender, ServiceStatus old, ServiceStatus value)
        {
            this.GetPipe(_provider, _provider.GetService<PrimaryScene>()).NetworkEntities.TryAdd(this.Entity);
            _provider = default;

            this.Entity.OnStatus[ServiceStatus.Initializing] += this.HandleStaticPipeEntityInitializing;
        }

        private void HandleStaticPipeEntityPipeChanged(INetworkEntity sender, IPipe old, IPipe value)
        {
            throw new AccessViolationException($"Invalid NetworkEntity.Pipe change detected. This should never happen once StaticPipeComponent has been registered to the entity.");
        }
        #endregion
    }
}
