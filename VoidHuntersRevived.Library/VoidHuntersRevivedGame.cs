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
using FarseerPhysics;
using VoidHuntersRevived.Library.Entities.Connections;

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
            stringLoader.Register("entity_name:tractor_beam", "Tractor Beam");
            stringLoader.Register("entity_description:tractor_beam", "A tractor beam.");

            stringLoader.Register("entity_name:wall", "Wall");
            stringLoader.Register("entity_description:wall", "A collection of Farseer rectangles forming an enclosed arena.");

            stringLoader.Register("entity_name:hull_square", "Hull Square");
            stringLoader.Register("entity_description:hull_square", "A Simple hull square.");

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<TractorBeam>("entity:tractor_beam", "entity_name:tractor_beam", "entity_description:tractor_beam");
            entityLoader.Register<Wall>("entity:wall", "entity_name:wall", "entity_description:wall");

            // Register all the default hull piece types
            entityLoader.Register<Hull>(
                handle: "entity:hull_square",
                nameHandle: "entity_name:hull_square",
                descriptionHandle: "entity_description:hull_square",
                data: new HullData(
                    maleConnection: new MaleConnection(new Vector2(-0.5f, 0), 0),
                    vertices: new Vector2[] {
                        new Vector2(-0.5f, -0.5f),
                        new Vector2(0.5f, -0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(-0.5f, 0.5f)
                    },
                    femaleConnections: new FemaleConnection[] {
                        new FemaleConnection(new Vector2(0.5f, 0), 0)
                    }));

            entityLoader.Register<Hull>(
                handle: "entity:hull_beam",
                nameHandle: "entity_name:hull_square",
                descriptionHandle: "entity_description:hull_square",
                data: new HullData(
                    maleConnection: new MaleConnection(new Vector2(-1.5f, 0), 0),
                    vertices: new Vector2[] {
                        new Vector2(-1.5f, -0.5f),
                        new Vector2(1.5f, -0.5f),
                        new Vector2(1.5f, 0.5f),
                        new Vector2(-1.5f, 0.5f)
                    },
                    femaleConnections: new FemaleConnection[] {
                    }));
        }
    }
}
