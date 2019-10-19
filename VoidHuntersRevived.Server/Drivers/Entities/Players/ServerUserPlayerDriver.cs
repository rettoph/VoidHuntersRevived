using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions;
using VoidHuntersRevived.Library.Utilities;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static VoidHuntersRevived.Library.Entities.Ship;

namespace VoidHuntersRevived.Server.Drivers.Entities.Players
{
    [IsDriver(typeof(UserPlayer))]
    public class ServerUserPlayerDrivers : Driver<UserPlayer>
    {
        #region Private FIelds
        private EntityCollection _entities;
        private ShipBuilder _builder;
        #endregion

        #region Constructor
        public ServerUserPlayerDrivers(ShipBuilder builder, EntityCollection entities, UserPlayer driven) : base(driven)
        {
            _builder = builder;
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("direction:changed:request", this.HandleDirectionChangedRequest);
            this.driven.Actions.TryAdd("tractor-beam:selected:request", this.HandleTractorBeamSelectedRequest);
            this.driven.Actions.TryAdd("tractor-beam:released:request", this.HandleTractorBeamReleasedRequest);
            this.driven.Actions.TryAdd("tractor-beam:attached:request", this.HandleTractorBeamAttachedRequest);
            this.driven.Actions.TryAdd("target:changed:request", this.HandleTargetChangedRequest);
            this.driven.Actions.TryAdd("firing:changed:request", this.HandleFiringChangedRequest);
            this.driven.Actions.TryAdd("self-destruct:request", this.HandleSelfDestructRequest);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(this.driven.Ship.Bridge == null)
            {
                using (FileStream import = File.OpenRead("Ships/mosquito.vh"))
                    this.driven.Ship.SetBridge(_builder.Import(import));
                
                var rand = new Random();
                this.driven.Ship.Bridge.SetPosition(new Vector2(rand.NextSingle(-100, 100), rand.NextSingle(-100, 100)), rand.NextSingle(-3, 3));
            }
        }
        #endregion

        #region Action Handlers
        private void HandleTargetChangedRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships direction.
                this.driven.Ship.ReadTargetOffset(im);
            }
        }

        private void HandleDirectionChangedRequest(object sender, NetIncomingMessage im)
        {
            if (this.ValidateSender(im))
            { // If the message checks out... update the ships direction.
                this.driven.Ship.SetDirection((Direction)im.ReadByte(), im.ReadBoolean());
            }
        }

        private void HandleTractorBeamSelectedRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
            {
                this.driven.Ship.ReadTargetOffset(arg);
                if(!this.driven.Ship.TractorBeam.TrySelect(_entities.GetById<ShipPart>(arg.ReadGuid())))
                { // Create a denied message & send to the client
                    this.driven.Actions.Create("tractor-beam:selected:denied", NetDeliveryMethod.ReliableOrdered, 1, arg.SenderConnection);
                }
            }
        }

        private void HandleTractorBeamReleasedRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
            {
                this.driven.Ship.ReadTargetOffset(arg);
                if (!this.driven.Ship.TractorBeam.TryRelease())
                { // Create a denied message & send to the client
                    this.driven.Actions.Create("tractor-beam:released:denied", NetDeliveryMethod.ReliableOrdered, 1, arg.SenderConnection);
                }
            }
        }

        private void HandleTractorBeamAttachedRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
            {
                // Attempt to attach the the object recieved by the client...
                if (!this.driven.Ship.TractorBeam.TryAttach(arg.ReadEntity<ShipPart>(_entities).FemaleConnectionNodes[arg.ReadInt32()]))
                { // Create a denied message & send to the client
                    this.driven.Actions.Create("tractor-beam:attached:denied", NetDeliveryMethod.ReliableOrdered, 1, arg.SenderConnection);
                }
            }
        }

        private void HandleFiringChangedRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
            {
                this.driven.Ship.SetFiring(arg.ReadBoolean());
                this.driven.Ship.ReadTargetOffset(arg);
            }
        }

        private void HandleSelfDestructRequest(object sender, NetIncomingMessage arg)
        {
            if (this.ValidateSender(arg))
            {
                this.driven.Ship.Bridge.Dispose();
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Validate that an incoming message was sent from the user
        /// belonging to the current user player. If it wasnt, kick the
        /// sender
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        private Boolean ValidateSender(NetIncomingMessage im)
        {
            if (im.SenderConnection == this.driven.User.Connection)
                return true;

            im.SenderConnection.Disconnect("Invalid message. Goodbye.");

            return false;
        }
        #endregion
    }
}