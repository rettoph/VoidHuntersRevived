using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Guppy.Extensions.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class MaleConnectionNode : ConnectionNode
    {
        private IServiceProvider _provider;

        public MaleConnectionNode(ShipPart parent, float rotation, Vector2 position, EntityConfiguration configuration, IServiceProvider provider, SpriteBatch spriteBatch = null) : base(parent, rotation, position, configuration, provider, spriteBatch)
        {
            _provider = provider;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.texture = _provider.GetLoader<ContentLoader>().Get<Texture2D>("texture:connection-node:male");

            this.SetDrawOrder(100);
        }
    }
}
