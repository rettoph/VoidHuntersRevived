using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class UserPlayer : Player
    {
        public IUser User;
        private IUserPlayerDriver _driver;

        public override String Name
        {
            get { return User.Name; }
        }

        public UserPlayer(IUser user, Hull bridge, EntityInfo info, IGame game) : base(bridge, info, game)
        {
            User = user;
        }
        public UserPlayer(long id, EntityInfo info, IGame game) : base(id, info, game)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Update the default driver for the current player isntance
            this.UpdateDriver();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _driver?.Update(gameTime);
        }

        private void UpdateDriver()
        {
            if (this.User != null)
            {
                if(_driver != null) // Remove the old driver
                    this.Scene.Entities.Remove(_driver);

                var group = (this.Scene as MainScene).Group;

                if (User.Id == group.Peer.UniqueIdentifier)
                { // Create a local driver
                    _driver = this.Scene.Entities.Create<IUserPlayerDriver>("entity:player_driver:local", null, this);
                }
                else
                { // Create a remote driver
                    _driver = this.Scene.Entities.Create<IUserPlayerDriver>("entity:player_driver:remote", null, this);
                }
            }
        }

        

        public override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            var group = (this.Scene as MainScene).Group;

            // Load the incoming user
            var userId = im.ReadInt64();

            if (User == null)
            { // If the user isnt already defined, define it now
                User = group.Users.GetById(userId);
                this.UpdateDriver();
            }

            if(userId != User.Id)
            { // If the claimed user is not the pre defined user, theres an issue
                this.Game.Logger.LogCritical($"Incorrect user claimed by player!");
            }


            if(im.ReadBoolean())
            {
                // read to the driver
                _driver.Read(im);
            }
        }

        public override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            om.Write(User.Id);

            if(_driver == null)
            { // Only sync driver info if the driver even exists at this time
                om.Write(false);
            }
            else
            { // Only sync driver info if the driver even exists at this time
                om.Write(true);

                // Write from the driver
                _driver.Write(om);
            }
        }
    }
}
