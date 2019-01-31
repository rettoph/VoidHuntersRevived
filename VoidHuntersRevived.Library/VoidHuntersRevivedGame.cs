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
using VoidHuntersRevived.Library.Entities.Connections.Nodes;
using VoidHuntersRevived.Networking;
using VoidHuntersRevived.Library.Entities.Players;
using FarseerPhysics.Collision.Shapes;
using VoidHuntersRevived.Library.Helpers;
using VoidHuntersRevived.Library.Extensions;
using VoidHuntersRevived.Library.Entities.Connections;

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

            stringLoader.Register("entity_name:player:user", "User Player");
            stringLoader.Register("entity_description:player:user", "A player controlled by a real human player.");

            stringLoader.Register("entity_name:tractor_beam", "Tractor Beam");
            stringLoader.Register("entity_description:tractor_beam", "A tractor beam.");

            stringLoader.Register("entity_name:wall", "Wall");
            stringLoader.Register("entity_description:wall", "A collection of Farseer rectangles forming an enclosed arena.");

            stringLoader.Register("entity_name:ship:user", "User Ship");
            stringLoader.Register("entity_description:ship:user", "A ship controllable by a user.");

            stringLoader.Register("entity_name:hull:square", "Hull Square");
            stringLoader.Register("entity_description:hull:square", "A Simple hull square.");

            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<UserPlayer>("entity:player:user", "entity_name:player:user", "entity_description:player:user");
            entityLoader.Register<TractorBeam>("entity:tractor_beam", "entity_name:tractor_beam", "entity_description:tractor_beam");
            entityLoader.Register<Wall>("entity:wall", "entity_name:wall", "entity_description:wall");
            
            entityLoader.Register<MaleConnectionNode>("entity:connection_node:male");
            entityLoader.Register<FemaleConnectionNode>("entity:connection_node:female");

            entityLoader.Register<TractorBeamConnection>("entity:connection:tractor_beam");
            entityLoader.Register<NodeConnection>("entity:connection:node");

            // Register all the default hull piece types
            entityLoader.Register<Hull>(
                handle: "entity:hull:square",
                nameHandle: "entity_name:hull:square",
                descriptionHandle: "entity_description:hull:square",
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
                handle: "entity:hull:beam",
                nameHandle: "entity_name:hull:square",
                descriptionHandle: "entity_description:hull:square",
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

            // https://www.desmos.com/calculator/akwfwhe8vp
            // Build triangle vertices with a side length of 1
            var d = (float)Math.Sqrt(1f / 3f) * 1;
            var nd = (float)(Math.Cos((1 * Math.PI) / 3) * d);

            entityLoader.Register<Hull>(
                handle: "entity:hull:triangle",
                nameHandle: "entity_name:hull:square",
                descriptionHandle: "entity_description:hull:square",
                data: new HullData(
                    maleConnection: Vector2Helper.FromThetaDistance((float)(3 * Math.PI) / 3, nd).ToVector3((float)(3 * Math.PI) / 3),
                    vertices: new Vector2[] {
                        Vector2Helper.FromThetaDistance((float)(0 * Math.PI) / 3, d),
                        Vector2Helper.FromThetaDistance((float)(2 * Math.PI) / 3, d),
                        Vector2Helper.FromThetaDistance((float)(4 * Math.PI) / 3, d),
                    },
                    femaleConnections: new Vector3[] {
                        Vector2Helper.FromThetaDistance((float)(1 * Math.PI) / 3, nd).ToVector3((float)(4 * Math.PI) / 3),
                        Vector2Helper.FromThetaDistance((float)(5 * Math.PI) / 3, nd).ToVector3((float)(2 * Math.PI) / 3),
                    }));
        }
    }
}
