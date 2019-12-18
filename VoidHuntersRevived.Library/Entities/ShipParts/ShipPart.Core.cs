using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Extensions.Collection;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// The core ShipPart class.
    /// 
    /// This will primarily augment the base
    /// FarseerEntity class.
    /// </summary>
    public partial class ShipPart : FarseerEntity
    {
        #region Protected Properties
        protected Color DefaultColor { get; set; } = Color.Orange;
        #endregion

        #region Public Propterties
        public Ship Ship { get; set; }
        public ShipPart Root { get => this.IsRoot ? this : this.Parent.Root; }
        public ShipPart Parent { get => this.MaleConnectionNode.Target?.Parent; }
        public Boolean IsRoot { get => !this.MaleConnectionNode.Attached; }
        public override Boolean IsActive { get => this.IsRoot; }
        public Color Color { get => this.Root.Ship == default(Ship) ? this.Root.DefaultColor : new Color(1, 203, 226); }
        public Single Health { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.ConnectionNode_Create(provider);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Transformations_PreInitialize();

            this.OnControllerChanged += this.HandleControllerChanged;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Health = 100;

            this.ConnectionNode_Initialize();
        }

        public override void Dispose()
        {
            // By default, ShipParts will automatically create an explosion & add themselves to
            // it when disposed, if their health is currently 0 & its not already in an explosion
            if (!(this.Controller is Explosion))
            {
                this.entities.Create<Explosion>("entity:explosion", e =>
                {
                    e.Add(this);
                });
            }

            // Continue the normal disposal process...
            base.Dispose();

            this.ConnectionNode_Dispose();
            this.Transformations_Dispose();

            this.OnControllerChanged -= this.HandleControllerChanged;
        }
        #endregion

        #region Farseer Methods
        public override Body CreateBody(World world)
        {
            var body = base.CreateBody(world);
            body.LinearDamping = 1f;
            body.AngularDamping = 1f;

            return body;
        }

        /// <summary>
        /// Update the input bodies world position based
        /// on the current ShipPart's transformation matrices
        /// with the assumtion that the inputed ShipPart is 
        /// the root ShipPart the local martrices are relative
        /// to.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="body"></param>
        public void SetWorldTransform(ShipPart root, Body body)
        {
            body.SetTransformIgnoreContacts(
                position: root.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(root.Rotation)),
                angle: root.Rotation + this.LocalRotation);
        }

        /// <summary>
        /// Damage the current ShipPart by a specific
        /// amount.
        /// </summary>
        /// <param name="amount"></param>
        public void Damage(Single amount)
        {
            this.Health -= amount;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Recursively populate the recieved list with all
        /// children of the current ShipPart's chain.
        /// </summary>
        /// <param name="list"></param>
        public void GetAllChildren(IList<ShipPart> list, Func<ShipPart, Boolean> filter = null)
        {
            if (filter == null || filter(this))
                list.Add(this);

            foreach (ConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.GetAllChildren(list, filter);
        }

        public IList<ShipPart> GetAllChildren(Func<ShipPart, Boolean> filter = null)
        {
            var list = new List<ShipPart>();

            this.GetAllChildren(list, filter);

            return list;
        }

        public Int32 GetSize()
        {
            var count = 0;
            this.GetSize(ref count);

            return count;
        }
        public void GetSize(ref Int32 count)
        {
            count++;

            foreach (ConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.GetSize(ref count);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When a ShipPart's controller is changed, we should
        /// automatically update the ShipPart's children's
        /// Controller as well
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleControllerChanged(object sender, Controller arg)
        {
            if (this.Status == InitializationStatus.Ready)
            { // Children should only inherit if the current controller is Ready
                this.FemaleConnectionNodes.ForEach(f =>
                {
                    if (f.Attached)
                        arg.Add(f.Target.Parent);
                });
            }
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ConnectionNode_Read(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.ConnectionNode_Write(om);
        }

        protected override void ReadVitals(NetIncomingMessage im)
        {
            base.ReadVitals(im);

            this.ReadHealth(im);
        }

        protected override void WriteVitals(NetOutgoingMessage om)
        {
            base.WriteVitals(om);

            this.WriteHealth(om);
        }

        /// <summary>
        /// Recursively read health from an incoming message
        /// </summary>
        /// <param name="im"></param>
        public void ReadHealth(NetIncomingMessage im)
        {
            this.Health = im.ReadSingle();

            while (im.ReadBoolean())
                this.FemaleConnectionNodes[im.ReadInt32()].Target.Parent.ReadHealth(im);
        }

        /// <summary>
        /// Recurively write health to an outbound message
        /// </summary>
        /// <param name="om"></param>
        public void WriteHealth(NetOutgoingMessage om)
        {
            om.Write(this.Health);

            this.FemaleConnectionNodes.ForEach(f =>
            {
                if(f.Attached)
                {
                    om.Write(true);
                    om.Write(f.Id);
                    f.Target.Parent.WriteHealth(om);
                }
            });

            om.Write(false);
        }
        #endregion
    }
}
