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

            // By default there is no parent
            this.SetParent(null);
        }
        #endregion

        #region Utility Methods
        internal void SetParent(ShipPart parent)
        {
            if (parent == null)
                this.logger.LogDebug($"Clearing {this.GetType().Name}({this.Id}) parent");
            else
                this.logger.LogDebug($"Setting {this.GetType().Name}({this.Id}) parent to {parent.GetType().Name}({parent.Id})");

            this.Parent = parent;

            this.UpdateRoot();
        }

        private void UpdateRoot()
        {
            // First clear the old fixture...
            if (this.Shape != null)
                this.DestroyFixture(this.Shape);

            if (this.Parent == null)
                this.Root = this;
            else
                this.Root = this.Parent.Root;

            // Now create a new fixture...
            this.Shape = this.Root.CreateFixture(_config.Shape.Clone() as PolygonShape);
        }
        #endregion
    }
}
