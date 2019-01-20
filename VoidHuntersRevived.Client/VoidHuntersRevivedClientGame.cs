using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Providers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoidHuntersRevived.Client
{
    class VoidHuntersRevivedClientGame : VoidHuntersRevivedGame
    {
        public VoidHuntersRevivedClientGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, IServiceCollection services = null) : base(logger, graphics, content, services)
        {
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            var content = this.Provider.GetLoader<ContentLoader>();
            content.Register<Texture2D>("basic_brick", "Sprites/basic-brick");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
