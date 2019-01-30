using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
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
            _user = group.Users.GetById(im.ReadInt64());

            this.UpdateDriver();
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);

            om.Write(_user.Id);
        }
    }
}
