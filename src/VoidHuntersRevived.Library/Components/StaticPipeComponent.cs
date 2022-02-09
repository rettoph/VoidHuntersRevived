using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Components
{
    /// <summary>
    /// Defines basic scaffolding for a component that will set an
    /// <see cref="IMagicNetworkEntity.Pipe"/> value on initialization.
    /// </summary>
    /// <typeparam name="TNetworkEntity"></typeparam>
    [HostTypeRequired(HostType.Remote)]
    public abstract class StaticPipeComponent<TNetworkEntity> : Component<TNetworkEntity>
        where TNetworkEntity : class, IMagicNetworkEntity
    {
        #region Lifeycycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Pipe = this.GetPipe(provider, provider.GetService<PrimaryScene>());

            this.Entity.OnPipeChanged += this.HandleStaticPipeEntityPipeChanged;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.OnPipeChanged -= this.HandleStaticPipeEntityPipeChanged;

            this.Entity.Pipe = default;
        }
        #endregion

        #region Helper Methods
        protected abstract Pipe GetPipe(ServiceProvider provider, PrimaryScene scene);
        #endregion

        #region Event Handlers

        private void HandleStaticPipeEntityPipeChanged(IMagicNetworkEntity sender, Pipe old, Pipe value)
        {
            throw new AccessViolationException($"Invalid NetworkEntity.Pipe change detected. This should never happen once StaticPipeComponent has been registered to the entity.");
        }
        #endregion
    }
}
