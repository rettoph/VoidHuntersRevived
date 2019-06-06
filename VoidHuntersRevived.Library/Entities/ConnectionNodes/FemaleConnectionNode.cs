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
    public class FemaleConnectionNode : ConnectionNode
    {
        private IServiceProvider _provider;

        public FemaleConnectionNode(ShipPart parent, float rotation, Vector2 position, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger, SpriteBatch spriteBatch = null) : base(parent, rotation, position, configuration, scene, provider, logger, spriteBatch)
        {
            _provider = provider;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.texture = _provider.GetLoader<ContentLoader>().Get<Texture2D>("texture:connection-node:female");

            this.SetDrawOrder(50);
        }
    }
}
