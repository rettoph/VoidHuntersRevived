using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Scenes
{
    class VoidHuntersServerWorldScene : VoidHuntersWorldScene
    {
        protected ServerPeer server;
        protected new ServerGroup group { get { return base.group as ServerGroup; } }

        public VoidHuntersServerWorldScene(ServerPeer server, World world, IServiceProvider provider) : base(server, world, provider)
        {
            this.server = server;
        }

        protected override void Boot()
        {
            base.Boot();

            this.group.MessageHandler.Add("action", this.HandleActionMessage);
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.group.Users.Added += this.HandleUserAdded;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.entities.Update(gameTime);
        }

        #region Event Handlers
        private void HandleUserAdded(object sender, User user)
        {
            /*
             * BEGIN NEW USER SETUP
             */
            // Cache all network entities as is
            var networkEntities = this.networkEntities.ToArray();

            // Send setup begin message to new user...
            this.group.SendMesssage(this.group.CreateMessage("setup:begin"), user, NetDeliveryMethod.ReliableOrdered);

            foreach (NetworkEntity ne in networkEntities.OrderBy(ne => ne.UpdateOrder))
                this.group.SendMesssage(ne.BuildCreateMessage(), user, NetDeliveryMethod.ReliableOrdered);

            // Send setup end message to new user...
            this.group.SendMesssage(this.group.CreateMessage("setup:end"), user, NetDeliveryMethod.ReliableOrdered);
            
            var bridge = this.entities.Create<ShipPart>("entity:ship-part");

            var player = this.entities.Create<Player>("entity:player");
            player.SetUser(user);
            player.SetBridge(bridge);
        }
        #endregion

        private void HandleActionMessage(NetIncomingMessage obj)
        {
            this.networkEntities.GetById(obj.ReadGuid()).HandleAction(obj.ReadString(), obj);
        }
    }
}
