using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Factories;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using Game = VoidHuntersRevived.Core.Implementations.Game;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using VoidHuntersRevived.Core.Loaders;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersRevivedGame : Game
    {
        public VoidHuntersRevivedGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, IServiceCollection services = null) : base(logger, graphics, content, services)
        {
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            var stringLoader = this.Provider.GetLoader<StringLoader>();
            stringLoader.Register("entity_name:brick", "Brick");
            stringLoader.Register("entity_description:brick", "A simple brick");

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<Brick>("entity_name:brick", "entity_description:brick");
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new game scene
            this.Scenes.Create<VoidHuntersScene>();
        }
    }
}
