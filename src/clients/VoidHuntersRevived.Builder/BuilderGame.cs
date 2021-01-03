using Guppy;
using Guppy.DependencyInjection;
using Guppy.Network.Peers;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.Scenes;
using VoidHuntersRevived.Client.Library;

namespace VoidHuntersRevived.Builder
{
    public sealed class BuilderGame : Game
    {
        #region Lifecycle Methods
        protected override void PreCreate(ServiceProvider provider)
        {
            base.PreCreate(provider);

            var client = provider.GetService<ClientPeer>();
            // _client.TryStart();
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Scenes.Create<ShipPartBuilderScene>();
        }
        #endregion
    }
}
