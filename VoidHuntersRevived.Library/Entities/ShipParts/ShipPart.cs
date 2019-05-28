using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Configurations;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public class ShipPart : FarseerEntity
    {
        #region Private Fields
        private ShipPartConfiguration _config;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current live shape used within the current
        /// parts fixture
        /// </summary>
        public PolygonShape Shape { get; private set; }

        public ShipPart Root { get; private set; }
        public ShipPart Parent { get; private set; }
        #endregion

        #region Constructors
        public ShipPart(EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
        }
        public ShipPart(Guid id, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, scene, provider, logger)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            _config = (ShipPartConfiguration)this.Configuration.Data;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.CreateFixture(_config.Shape);
        }
        #endregion
    }
}
