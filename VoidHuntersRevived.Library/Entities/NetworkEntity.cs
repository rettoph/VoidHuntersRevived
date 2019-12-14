using Guppy;
using Guppy.Collections;
using Guppy.Network.Groups;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities.Delegaters;

namespace VoidHuntersRevived.Library.Entities
{
    public class NetworkEntity : Entity
    {
        #region Public Properties
        public ActionMessageDelegater Actions { get; private set; }
        /// <summary>
        /// Indicates that the current ShipPart is dirty 
        /// and should be cleaned next frame.
        /// </summary>
        public Boolean Dirty { get; private set; }
        #endregion

        #region Protected Fields
        protected EntityCollection entities { get; private set; }
        protected Group group { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Create a new action delegater
            this.Actions = ActivatorUtilities.CreateInstance<ActionMessageDelegater>(provider, this);

            this.entities = provider.GetRequiredService<EntityCollection>();
            this.group = provider.GetRequiredService<NetworkScene>().Group;

            this.Events.Register<Boolean>("dirty:changed");
            this.Events.Register<GameTime>("clean");
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Dirty = true;
        }

        public override void Dispose()
        {
            base.Dispose();

            // Dispose of the earlier delegater
            this.Actions.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Dirty)
            { // If the curent entity is dirty...
                this.Events.TryInvoke<GameTime>(this, "clean", gameTime);
                this.SetDirty(false);
            }
        }
        #endregion

        #region Helper Methods
        public void SetDirty(Boolean value)
        {
            if (this.Dirty != value)
            {
                this.Dirty = value;

                this.Events.TryInvoke<Boolean>(this, "dirty:changed", this.Dirty);
            }
        }
        #endregion

        #region Network Methods
        /// <summary>
        /// Read setup data
        /// </summary>
        /// <param name="im"></param>
        public virtual void TryReadSetup(NetIncomingMessage im)
        {
            this.ReadSetup(im);
        }

        /// <summary>
        /// Write setup data
        /// </summary>
        /// <param name="im"></param>
        public virtual void TryWriteSetup(NetOutgoingMessage om)
        {
            this.WriteSetup(om);
        }

        /// <summary>
        /// Read the full entity network data
        /// </summary>
        /// <param name="im"></param>
        public virtual void TryRead(NetIncomingMessage im)
        {
            this.Read(im);
        }

        /// <summary>
        /// Write the full entity network data
        /// </summary>
        /// <param name="om"></param>
        public virtual void TryWrite(NetOutgoingMessage om)
        {
            this.Write(om);
        }

        /// <summary>
        /// Read setup data
        /// </summary>
        /// <param name="im"></param>
        protected virtual void ReadSetup(NetIncomingMessage im)
        {
            //
        }

        /// <summary>
        /// Write setup data
        /// </summary>
        /// <param name="im"></param>
        protected virtual void WriteSetup(NetOutgoingMessage om)
        {
            //
        }

        /// <summary>
        /// Read the full entity network data
        /// </summary>
        /// <param name="im"></param>
        protected virtual void Read(NetIncomingMessage im)
        {
            //
        }

        /// <summary>
        /// Write the full entity network data
        /// </summary>
        /// <param name="om"></param>
        protected virtual void Write(NetOutgoingMessage om)
        {
            //
        }
        #endregion
    }
}
