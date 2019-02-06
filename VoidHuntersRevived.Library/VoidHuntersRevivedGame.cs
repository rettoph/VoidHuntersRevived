using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using VoidHuntersRevived.Networking;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Loaders;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.MetaData;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Helpers;
using VoidHuntersRevived.Library.Extensions;

namespace VoidHuntersRevived.Library
{
    /// <summary>
    /// The main game class containig shared functionality between
    /// client and server peers
    /// </summary>
    public class VoidHuntersRevivedGame : NetworkGame
    {
        #region Constructors
        public VoidHuntersRevivedGame(
            ILogger logger,
            GraphicsDeviceManager graphics = null,
            ContentManager content = null,
            GameWindow window = null,
            IServiceCollection services = null)
            : base(logger, graphics, content, window, services)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Register global game strings
            var stringLoader = this.Provider.GetLoader<StringLoader>();
            stringLoader.Register("entity_name:default", "Unnamed");
            stringLoader.Register("entity_description:default", "An unnamed entity");
            stringLoader.Register("entity_name:hull:square", "Hull Square");
            stringLoader.Register("entity_description:hull:square", "A Hull Square");

            // Register global game entities
            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<NodeConnection>("entity:connection:connection_node");
            entityLoader.Register<MaleConnectionNode>("entity:connection_node:male");
            entityLoader.Register<FemaleConnectionNode>("entity:connection_node:female");
            entityLoader.Register<TractorBeamConnection>("entity:connection:tractor_beam");
            entityLoader.Register<TractorBeam>("entity:tractor_beam");
            entityLoader.Register<UserPlayer>("entity:player:user");

            entityLoader.Register<ShipPart>(
                handle: "entity:hull:square",
                nameHandle: "entity_name:hull:square",
                descriptionHandle: "entity_description:hull:square",
                data: new ShipPartData(
                    maleConnectionNodeData: new Vector3(-0.5f, 0, (float)Math.PI),
                    vertices: new Vector2[] {
                        new Vector2(-0.5f, -0.5f),
                        new Vector2(0.5f, -0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(-0.5f, 0.5f)
                    },
                    femaleConnectionNodesData: new Vector3[] {
                        new Vector3(0.5f, 0, (float)Math.PI),
                        new Vector3(0f, -0.5f, (float)Math.PI/2),
                        new Vector3(0f, 0.5f, -(float)Math.PI/2)
                    }));

            entityLoader.Register<ShipPart>(
                handle: "entity:hull:beam",
                nameHandle: "entity_name:hull:square",
                descriptionHandle: "entity_description:hull:square",
                data: new ShipPartData(
                    maleConnectionNodeData: new Vector3(-1.5f, 0, (float)Math.PI),
                    vertices: new Vector2[] {
                        new Vector2(-1.5f, -0.5f),
                        new Vector2(1.5f, -0.5f),
                        new Vector2(1.5f, 0.5f),
                        new Vector2(-1.5f, 0.5f)
                    },
                    femaleConnectionNodesData: new Vector3[] {
                        new Vector3(1.5f, 0, (float)Math.PI),
                        new Vector3(-1f, -0.5f, (float)Math.PI/2),
                        new Vector3(0f, -0.5f, (float)Math.PI/2),
                        new Vector3(1f, -0.5f, (float)Math.PI/2),
                        new Vector3(-1f, 0.5f, -(float)Math.PI/2),
                        new Vector3(0f, 0.5f, -(float)Math.PI/2),
                        new Vector3(1f, 0.5f, -(float)Math.PI/2),
                    }));

            // https://www.desmos.com/calculator/akwfwhe8vp
            // Build triangle vertices with a side length of 1
            var d = (float)Math.Sqrt(1f / 3f) * 1;
            var nd = (float)(Math.Cos((1 * Math.PI) / 3) * d);

            entityLoader.Register<ShipPart>(
                handle: "entity:hull:triangle",
                nameHandle: "entity_name:hull:square",
                descriptionHandle: "entity_description:hull:square",
                data: new ShipPartData(
                    maleConnectionNodeData: Vector2Helper.FromThetaDistance((float)(3 * Math.PI) / 3, nd).ToVector3((float)(3 * Math.PI) / 3),
                    vertices: new Vector2[] {
                        Vector2Helper.FromThetaDistance((float)(0 * Math.PI) / 3, d),
                        Vector2Helper.FromThetaDistance((float)(2 * Math.PI) / 3, d),
                        Vector2Helper.FromThetaDistance((float)(4 * Math.PI) / 3, d),
                    },
                    femaleConnectionNodesData: new Vector3[] {
                        Vector2Helper.FromThetaDistance((float)(1 * Math.PI) / 3, nd).ToVector3((float)(4 * Math.PI) / 3),
                        Vector2Helper.FromThetaDistance((float)(5 * Math.PI) / 3, nd).ToVector3((float)(2 * Math.PI) / 3),
                    }));
        }
        #endregion
    }
}
