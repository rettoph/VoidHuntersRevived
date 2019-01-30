using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class UserPlayer : NetworkEntity, IPlayer
    {
        private IUser _user;
        private IUserPlayerDriver _driver;

        public String Name
        {
            get { return _user.Name; }
        }
        public TractorBeam TractorBeam { get; private set; }

        public UserPlayer(IUser user, EntityInfo info, IGame game) : base(info, game)
        {
            _user = user;

            this.Enabled = true;
        }
        public UserPlayer(long id, EntityInfo info, IGame game) : base(id, info, game)
        {
            this.Enabled = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new tractorbeam for the player
            this.TractorBeam = this.Scene.Entities.Create<TractorBeam>("entity:tractor_beam", null, this);

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
            if (_user != null)
            {
                var group = (this.Scene as MainScene).Group;

                if (_user.Id == group.Peer.UniqueIdentifier)
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
            var group = (this.Scene as MainScene).Group;

            // Load the incoming user
            var userId = im.ReadInt64();

            if (_user == null)
            { // If the user isnt already defined, define it now
                _user = group.Users.GetById(userId);
                this.UpdateDriver();
            }

            if(userId != _user.Id)
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
            om.Write(this.Id);

            // Write the user id
            om.Write(_user.Id);

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
