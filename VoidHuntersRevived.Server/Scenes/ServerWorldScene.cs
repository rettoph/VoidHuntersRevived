using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Security;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.System;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerWorldScene : WorldScene
    {
        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.Group.Users.Events.TryAdd<User>("added", this.HandleUserJoined);

            var rand = new Random();
            for(Int32 i=0; i<10; i++)
            {
                this.entities.Create<ShipPart>("entity:ship-part:hull:triangle").Body.SetTransformIgnoreContacts(rand.NextVector2(-30, 30), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:hull:square").Body.SetTransformIgnoreContacts(rand.NextVector2(-30, 30), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:hull:hexagon").Body.SetTransformIgnoreContacts(rand.NextVector2(-30, 30), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
            }
        }
        #endregion

        #region Event Handlers
        private void HandleUserJoined(object sender, User arg)
        {
            this.entities.Create<UserPlayer>("entity:player:user", p => {
                p.User = arg;
                p.SetShip(this.entities.Create<Ship>("entity:ship", s =>
                {
                    s.SetBridge(this.entities.Create<ShipPart>("entity:ship-part:hull:square"));
                }));
            });
        }
        #endregion
    }
}
