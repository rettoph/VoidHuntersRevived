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
        }
        #endregion
    }
}
