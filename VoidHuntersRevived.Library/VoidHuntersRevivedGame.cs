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
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Networking;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersRevivedGame : NetworkGame
    {
        public VoidHuntersRevivedGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, GameWindow window = null, IServiceCollection services = null) : base(logger, graphics, content, window, services)
        {
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            var stringLoader = this.Provider.GetLoader<StringLoader>();
            stringLoader.Register("entity_name:default", "Unnamed");
            stringLoader.Register("entity_description:default", "An unnamed entity.");

            stringLoader.Register("entity_name:tractor_beam", "Tractor Beam");
            stringLoader.Register("entity_description:tractor_beam", "A tractor beam.");

            stringLoader.Register("entity_name:wall", "Wall");
            stringLoader.Register("entity_description:wall", "A collection of Farseer rectangles forming an enclosed arena.");

            stringLoader.Register("entity_name:ship:user", "User Ship");
            stringLoader.Register("entity_description:ship:user", "A ship controllable by a user.");

            stringLoader.Register("entity_name:hull_square", "Hull Square");
            stringLoader.Register("entity_description:hull_square", "A Simple hull square.");

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<TractorBeam>("entity:tractor_beam", "entity_name:tractor_beam", "entity_description:tractor_beam");
            entityLoader.Register<Wall>("entity:wall", "entity_name:wall", "entity_description:wall");
            entityLoader.Register<UserShip>("entity:ship:user", "entity_name:ship:user", "entity_description:ship:user");
            entityLoader.Register<MaleConnectionNode>("entity:connection_node:male");
            entityLoader.Register<FemaleConnectionNode>("entity:connection_node:female");

            // Register all the default hull piece types
            entityLoader.Register<Hull>(
                handle: "entity:hull_square",
                nameHandle: "entity_name:hull_square",
                descriptionHandle: "entity_description:hull_square",
                data: new HullData(
                    maleConnection: new Vector3(-0.5f, 0, (float)Math.PI),
                    vertices: new Vector2[] {
                        new Vector2(-0.5f, -0.5f),
                        new Vector2(0.5f, -0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(-0.5f, 0.5f)
                    },
                    femaleConnections: new Vector3[] {
                        new Vector3(0.5f, 0, 0),
                        new Vector3(0f, -0.5f, -(float)Math.PI/2),
                        new Vector3(0f, 0.5f, (float)Math.PI/2)
                    }));

            entityLoader.Register<Hull>(
                handle: "entity:hull_beam",
                nameHandle: "entity_name:hull_square",
                descriptionHandle: "entity_description:hull_square",
                data: new HullData(
                    maleConnection: new Vector3(-1.5f, 0, (float)Math.PI),
                    vertices: new Vector2[] {
                        new Vector2(-1.5f, -0.5f),
                        new Vector2(1.5f, -0.5f),
                        new Vector2(1.5f, 0.5f),
                        new Vector2(-1.5f, 0.5f)
                    },
                    femaleConnections: new Vector3[] {
                        new Vector3(1.5f, 0, 0),
                        new Vector3(-1f, -0.5f, -(float)Math.PI/2),
                        new Vector3(0f, -0.5f, -(float)Math.PI/2),
                        new Vector3(1f, -0.5f, -(float)Math.PI/2),
                        new Vector3(-1f, 0.5f, (float)Math.PI/2),
                        new Vector3(0f, 0.5f, (float)Math.PI/2),
                        new Vector3(1f, 0.5f, (float)Math.PI/2),
                    }));
        }
    }
}
