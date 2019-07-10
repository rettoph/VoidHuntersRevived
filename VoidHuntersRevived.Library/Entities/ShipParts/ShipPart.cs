using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Utilities.ConnectionNodes;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public abstract partial class ShipPart : FarseerEntity
    {
        #region Protected Attributes
        protected ShipPartConfiguration config { get; private set; }
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current live shape used within the current
        /// parts fixture
        /// </summary>
        public Fixture Fixture { get; protected set; }

        public Player BridgeFor { get; internal set; }
        public Boolean IsBridge { get { return this.BridgeFor != null; } }

        public ShipPart Root { get { return this.Parent == null ? this : this.Parent.Root; } }
        public Boolean IsRoot { get { return this.Parent == null; } }

        public ShipPart Parent { get { return this.MaleConnectionNode.Target?.Parent; } }
        #endregion

        #region Constructors
        public ShipPart(EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
        }
        public ShipPart(Guid id, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            this.config = (ShipPartConfiguration)this.Configuration.Data;

            // Call the internal connection node boot method
            this.ConnectionNodes_Boot();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Fixture = this.CreateFixture(this.config.Shape, this);

            this.CollisionCategories = Category.Cat2;
            this.CollidesWith = Category.Cat1;

            this.SetUpdateOrder(100);
        }
        #endregion

        #region Frame Methods
        protected override void draw(GameTime gameTime)
        {
            #if DEBUG
            MaleConnectionNode.Draw(gameTime);

            foreach(FemaleConnectionNode female in this.FemaleConnectionNodes)
                female.Draw(gameTime);
            #endif
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Update the internal ship part's placement in its current chain.
        /// </summary>
        protected abstract void UpdateChainPlacement();
        #endregion
    }
}
