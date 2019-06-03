﻿using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart : FarseerEntity
    {
        #region Private Fields
        private EntityCollection _entities;
        private ShipPartConfiguration _config;
        private MaleConnectionNode _maleConnectionNode;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current live shape used within the current
        /// parts fixture
        /// </summary>
        public PolygonShape Shape { get; private set; }

        public Player BridgeFor { get; internal set; }
        public Boolean IsBridge { get { return this.BridgeFor != null; } }

        public ShipPart Root { get { return this.Parent == null ? this : this.Parent.Root; } }
        public Boolean IsRoot { get { return this.Parent == null; } }

        public ShipPart Parent { get; internal set; }
        #endregion

        #region Constructors
        public ShipPart(EntityCollection entities, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
            _entities = entities;
        }
        public ShipPart(Guid id, EntityCollection entities, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, scene, provider, logger)
        {
            _entities = entities;
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            _config = (ShipPartConfiguration)this.Configuration.Data;
            _maleConnectionNode = _entities.Create<MaleConnectionNode>(
                "connection-node:male",
                this, 
                _config.MaleConnectionNode.Z,
                new Vector2(_config.MaleConnectionNode.X, _config.MaleConnectionNode.Y));
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.CreateFixture(_config.Shape, this);

            this.CollisionCategories = Category.Cat2;
            this.CollidesWith = Category.Cat1;
        }
        #endregion
    }
}
