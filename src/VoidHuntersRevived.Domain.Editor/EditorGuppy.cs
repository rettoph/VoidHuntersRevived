using Guppy.Common;
using Guppy.MonoGame;
using Guppy.MonoGame.UI;
using Guppy.Network;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Peers;
using Guppy.Providers;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Client;
using VoidHuntersRevived.Domain.Server;

namespace VoidHuntersRevived.Domain.Editor
{
    public class EditorGuppy : ClientGameGuppy
    {
        private readonly IShipPartEditor[] _editors;
        private readonly ShipPartResource _shipPart;
        private readonly ServerGameGuppy _server;

        public EditorGuppy(
            ClientPeer client, 
            NetScope netScope, 
            ISimulationService simulations,
            IFiltered<IShipPartEditor> editors,
            IGuppyProvider guppies) : base(client, netScope, simulations)
        {
            _editors = editors.Instances.ToArray();
            _shipPart = new ShipPartResource("editing");
            _server = guppies.Create<ServerGameGuppy>();
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            foreach (var editor in _editors)
            {
                editor.Initialize(_shipPart);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _server.Dispose();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (var editor in _editors)
            {
                editor.Draw(gameTime);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var editor in _editors)
            {
                editor.Update(gameTime);
            }
        }
    }
}