using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Lidgren.Network;
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

        public ShipPart Root { get { return this.Parent == null ? this : this.Parent.Root; } }
        public Boolean IsRoot { get { return this.Parent == null; } }

        public Ship BridgeFor { get; internal set; }
        public Boolean IsBridge { get { return this.BridgeFor != null; } }

        public ShipPart Parent { get { return this.MaleConnectionNode.Target?.Parent; } }

        public Vector2 Centeroid { get { return this.config.Centeroid; } }

        public Vector2 WorldCenteroid
        {
            get
            {
                return this.Position + Vector2.Transform(this.Centeroid, Matrix.CreateRotationZ(this.Rotation));
            }
        }

        public new Vector2 WorldCenter
        {
            get
            {
                return (this.IsRoot ? base.WorldCenter : this.Root.WorldCenter);
            }
            set
            {
                if (this.IsRoot)
                {
                    base.Position = value;
                }
                else
                {
                    throw new Exception("Unable to set ShipPart world center when not root.");
                }
            }
        }

        public new Vector2 Position
        {
            get
            {
                return (this.IsRoot ? base.Position : this.Root.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(this.Root.Rotation)));
            }
            set
            {
                if(this.IsRoot)
                {
                    base.Position = value;
                }
                else
                {
                    throw new Exception("Unable to set ShipPart position when not root.");
                }
            }
        }

        public new Single Rotation
        {
            get
            {
                return (this.IsRoot ? base.Rotation : this.Root.Rotation + this.LocalRotation);
            }
            set
            {
                if (this.IsRoot)
                {
                    base.Rotation = value;
                }
                else
                {
                    throw new Exception("Unable to set ShipPart rotation when not root.");
                }
            }
        }
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

            this.Fixture = this.CreateFixture(new PolygonShape(this.config.Vertices, this.config.Density), this);

            this.CollisionCategories = Category.Cat2;
            this.CollidesWith = Category.Cat1;

            this.SetUpdateOrder(50);
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

        #region Helper Methods
        public List<ShipPart> GetChildren()
        {
            var children = new List<ShipPart>();

            this.GetChildren(ref children);

            return children;
        }

        public void GetChildren(ref List<ShipPart> children)
        {
            children.Add(this);

            foreach (FemaleConnectionNode female in this.FemaleConnectionNodes)
                if (female.Connected)
                    female.Target.Parent.GetChildren(ref children);
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Update the internal ship part's placement in its current chain.
        /// </summary>
        protected abstract void UpdateChainPlacement();
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            base.read(im);

            this.ConnectionNodes_Read(im);
        }

        protected override void write(NetOutgoingMessage om)
        {
            base.write(om);

            this.ConnectionNodes_Write(om);
        }
        #endregion
    }
}
