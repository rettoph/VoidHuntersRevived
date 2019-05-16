using Guppy;
using Guppy.Configurations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class Player : NetworkEntity
    {
        public User User { get; private set; }

        public Player(User user, EntityConfiguration configuration, Scene scene, ILogger logger) : base(configuration, scene, logger)
        {
            this.User = user;
        }

        public Player(Guid id, EntityConfiguration configuration, Scene scene, ILogger logger) : base(id, configuration, scene, logger)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            // Load the player's user by id...
            this.User = (this.scene as VoidHuntersWorldScene).Users.GetById(im.ReadGuid());
        }

        public override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            // Write the player's user id...
            om.Write(this.User.Id);
        }
    }
}
