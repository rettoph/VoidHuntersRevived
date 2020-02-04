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
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Extensions.Entities.ShipParts;
using FarseerPhysics.Common;
using VoidHuntersRevived.Library.Entities.Players;

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
        public Color Color { get => this.Root.Ship == default(Ship) || this.Root.Ship.Player == default(Player) ? this.Root.DefaultColor : this.Root.Ship.Player.Team.Color; }
        public Byte Health { get; internal set; }
        public Single HealthRate { get => (Single)this.Health / 100; }
        public ShipPartConfiguration Configuration { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Invoked when the current ShipPart's health is updated...
        /// </summary>
        public event EventHandler<Byte> OnHealthChanged;
        /// <summary>
        /// Invoked when any ShipPart within the current ShipPart's chain's
        /// health is updated, including the current ShipPart.
        /// </summary>
        public event EventHandler<ShipPart> OnChildHealthChanged;
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

            this.ConnectionNode_Dispose();
            this.Transformations_Dispose();

            // Continue the normal disposal process...
            base.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Children.TryDrawAll(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Children.TryUpdateAll(gameTime);
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
        public void Damage(Byte amount)
        {
            this.Health -= amount;

            this.OnHealthChanged?.Invoke(this, this.Health);
            this.Root.OnChildHealthChanged?.Invoke(this, this);
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

        public void GetInfo(out String title, out String description, out String advanced)
        {
            if (this.IsRoot)
            {
                title = $"{this.Name}";
                description = $"{this.Description}";
                advanced = $"Health: {this.Health}/100";

                if (this.GetSize() > 1)
                    advanced += $"\nChain Size: {this.GetSize()}";
            }
            else
            {
                this.Root.GetInfo(out title, out description, out advanced);
            }
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ConnectionNode_Read(im);

            this.ReadHealth(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.ConnectionNode_Write(om);

            this.WriteHealth(om);
        }


        public override bool CanSendVitals(bool interval)
        {
            return this.IsRoot && base.CanSendVitals(interval);
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
        #endregion
    }
}
