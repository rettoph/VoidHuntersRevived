using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Scenes;
using Guppy.Network.Peers;
using Guppy.Network.Security;
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
            this.entities.Create("farseer-entity");
        }
        #endregion
    }
}
