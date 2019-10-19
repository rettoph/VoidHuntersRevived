using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.Collection;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Contains miscellaneous code not specific to any other
    /// partial classes.
    /// </summary>
    public abstract partial class ShipPart
    {
        #region Enums
        public enum State
        {
            Active,
            Passive
        }
        #endregion

        #region Protected Fields
        protected ShipPartConfiguration config;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The ship the current ship part is a bridge for, if any.
        /// </summary>
        public Ship BridgeFor { get; internal set; }

        /// <summary>
        /// If the current shippart is a bridge
        /// </summary>
        public Boolean IsBridge { get { return this.BridgeFor != null; } }

        public Vector2 Centeroid { get => this.config.Centeroid; }
        public Vector2 LocalCenteroid { get=> Vector2.Transform(Vector2.Zero, this.LocalTransformation) + Vector2.Transform(this.Centeroid, Matrix.CreateRotationZ(this.LocalRotation)); }
        public Vector2 WorldCenteroid { get => this.Position + Vector2.Transform(this.Centeroid, Matrix.CreateRotationZ(this.Rotation)); }

        /// <summary>
        /// Live ShipPart's are shipparts that can self updated when connected to a ship.
        /// 
        /// Byt default, all children within a ship are disabled and will not be updated,
        /// but any ShipPart marked as live will be updated each frame.
        /// 
        /// These include weapons, thrusters, and more.
        /// </summary>
        public virtual Boolean IsLive { get; protected set; }

        public Single Health { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Call internal create functions
            this.ConnectionNode_Create(provider);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Save the configuration
            this.config = this.Configuration.Data as ShipPartConfiguration;

            // Call internal pre initialize functions
            this.ConnectionNode_PreInitialize();
            this.Transformations_PreInitialize();
            this.Farseer_PreInitialize();

            this.Health = 100;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.SetEnabled(false);

            this.ConnectionNode_Initialize();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();
        }

        public override void Dispose()
        {
            base.Dispose();

            // Call internal dispose functions
            this.ConnectionNode_Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.ConnectionNode_Update(gameTime);

            for(Int32 i=0; i<this.FemaleConnectionNodes.Length; i++)
                this.FemaleConnectionNodes[i].Target?.Parent.Update(gameTime);

            if (this.Health < 100)
                this.Health += 0.001f * (Single)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Recursively populate the recieved list with all
        /// children of the current ShipPart's chain.
        /// </summary>
        /// <param name="list"></param>
        public void GetAllChildren(ref List<ShipPart> list,Func<ShipPart, Boolean> filter = null)
        {
            if(filter == null || filter(this))
                list.Add(this);

            foreach (FemaleConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.GetAllChildren(ref list, filter);
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

            foreach (FemaleConnectionNode female in this.FemaleConnectionNodes)
                if (female.Attached)
                    female.Target.Parent.GetSize(ref count);
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            // Read vital data
            this.body.ReadPosition(im);
            this.body.ReadVelocity(im);

            this.ConnectionNode_Read(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            // Write vital data
            this.body.WritePosition(om);
            this.body.WriteVelocity(om);

            this.ConnectionNode_Write(om);
        }

        protected override void ReadPostInitialize(NetIncomingMessage im)
        {
            base.ReadPostInitialize(im);

            this.SetPosition(im.ReadVector2(), im.ReadSingle());
        }

        protected override void WritePostInitialize(NetOutgoingMessage om)
        {
            base.WritePostInitialize(om);

            om.Write(this.Position);
            om.Write(this.Rotation);
        }
        #endregion
    }
}
