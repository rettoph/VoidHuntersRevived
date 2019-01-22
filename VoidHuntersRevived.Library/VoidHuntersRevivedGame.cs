using Game = VoidHuntersRevived.Core.Implementations.Game;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Loaders;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersRevivedGame : Game
    {
        public VoidHuntersRevivedGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, GameWindow window = null, IServiceCollection services = null) : base(logger, graphics, content, window, services)
        {
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            var stringLoader = this.Provider.GetLoader<StringLoader>();
            stringLoader.Register("entity_name:wall", "Wall");
            stringLoader.Register("entity_description:wall", "A collection of Farseer rectangles forming an enclosed arena.");

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<Wall>("entity:wall", "entity_name:wall", "entity_description:wall");
        }
    }
}
