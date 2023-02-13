using Guppy.Common;
using Guppy.MonoGame;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.UI;
using Guppy.Network;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Peers;
using Guppy.Providers;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Client;
using VoidHuntersRevived.Domain.Server;

namespace VoidHuntersRevived.Domain.Editor
{
    public class EditorGuppy : LocalGameGuppy
    {
        private readonly IShipPartEditor[] _editors;
        private readonly ShipPartResource _shipPart;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        public EditorGuppy(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            ISimulationService simulations,
            IFiltered<IShipPartEditor> editors,
            IGuppyProvider guppies) : base(simulations)
        {
            _editors = editors.Instances.ToArray();
            _shipPart = new ShipPartResource("editing");
            _primitiveBatch = primitiveBatch;
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            foreach (var editor in _editors)
            {
                editor.Initialize(_shipPart);
            }
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