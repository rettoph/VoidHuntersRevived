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
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using FarseerPhysics.Common;
using VoidHuntersRevived.Library.Entities.MetaData;

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

            stringLoader.Register("entity_name:ship", "Ship");
            stringLoader.Register("entity_description:ship", "A Ship.");

            stringLoader.Register("entity_name:tractor_beam", "Tractor Beam");
            stringLoader.Register("entity_description:tractor_beam", "A Tractor Beam.");

            stringLoader.Register("entity_name:hull_square", "Hull Square");
            stringLoader.Register("entity_description:hull_square", "A Simple hull square.");

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<Wall>("entity:wall", "entity_name:wall", "entity_description:wall");
            entityLoader.Register<Ship>("entity:ship", "entity_name:ship", "entity_description:ship");
            entityLoader.Register<TractorBeam>("entity:tractor_beam", "entity_name:tractor_beam", "entity_description:tractor_beam");

            // Register all the default hull piece types
            entityLoader.Register<Hull>(
                handle: "entity:hull_square",
                nameHandle: "entity_name:hull_square",
                descriptionHandle: "entity_description:hull_square",
                data: new HullData(
                    vertices: new Vector2[] {
                        new Vector2(-0.5f, -0.5f),
                        new Vector2(0.5f, -0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(-0.5f, 0.5f)
                    }));
        }
    }
}
