using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.Players;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Scenes;
using Guppy.Collections;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Server.Scenes
{
    internal sealed class ServerGalacticFightersWorldScene : GalacticFightersWorldScene
    {
        #region Constructor
        public ServerGalacticFightersWorldScene(World world) : base(world)
        {

        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.Group.Users.Events.TryAdd<User>("added", this.HandleUserJoined);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.entities.TryUpdate(gameTime);
        }
        #endregion

        #region Event Handlers
        private void HandleUserJoined(object sender, User arg)
        {
            // Create a new player instance for the new user
            this.entities.Create<UserPlayer>("player:user", player =>
            {
                player.User = arg;
                player.Ship = this.entities.Create<Ship>("ship", ship =>
                { // Build a new ship for the player...
                    if(ship.Bridge == null)
                    { // Build a new bridge for the ship if one is not already set...
                        ship.SetBridge(this.entities.Create<ShipPart>("ship-part:triangle"));
                    }

                });
            });

            // for(Int32 i=0; i<10; i++)
            //     this.entities.Create<ShipPart>("ship-part:triangle");
        }
        #endregion
    }
}
