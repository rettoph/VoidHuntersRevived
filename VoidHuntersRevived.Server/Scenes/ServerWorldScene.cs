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
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerWorldScene : WorldScene
    {
        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Layer 0: Default
            this.layers.Create<BasicLayer>(0, l =>
            {
                l.SetUpdateOrder(10);
                l.SetDrawOrder(20);
            });
            // Layer 1: Chunk
            this.layers.Create<BasicLayer>(1, l =>
            {
                l.SetUpdateOrder(20);
                l.SetDrawOrder(10);
            });

            this.Group.Users.Events.TryAdd<User>("added", this.HandleUserJoined);

            var rand = new Random();
            var size = 200;
            for(Int32 i=0; i<250; i++)
            {
                this.entities.Create<ShipPart>("entity:ship-part:hull:triangle").Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:hull:square").Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:hull:hexagon").Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small").Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small").Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small").Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small").Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small").Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
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
                    s.SetBridge(this.entities.Create<ShipPart>("entity:ship-part:chassis:mosquito"));
                }));
            });
        }
        #endregion
    }
}
