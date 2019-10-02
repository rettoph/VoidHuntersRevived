using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.Players;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Extensions;
using GalacticFighters.Library.Scenes;
using Guppy.Collections;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Server.Scenes
{
    internal sealed class ServerGalacticFightersWorldScene : GalacticFightersWorldScene
    {
        #region Private Fields
        private Queue<User> _newUsers;
        #endregion

        #region Protected Fields
        protected Random random { get; private set; }
        #endregion

        #region Constructor
        public ServerGalacticFightersWorldScene(World world) : base(world)
        {

        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _newUsers = new Queue<User>();

            this.random = provider.GetRequiredService<Random>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Group.Users.Events.TryAdd<User>("added", this.HandleUserJoined);
        }

        public override void Dispose()
        {
            base.Dispose();

            _newUsers.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            lock (_newUsers)
                while (_newUsers.Any())
                    this.AddUser(_newUsers.Dequeue());

            this.entities.TryUpdate(gameTime);
        }
        #endregion

        /// <summary>
        /// Take a user object & create assign a ship to them
        /// </summary>
        /// <param name="user"></param>
        private void AddUser(User user)
        {
            // Create a new player instance for the new user
            this.entities.Create<UserPlayer>("player:user", player =>
            {
                player.User = user;
                player.Ship = this.entities.Create<Ship>("ship", ship =>
                { // Build a new ship for the player...
                    if (ship.Bridge == null)
                    { // Build a new bridge for the ship if one is not already set...
                        ship.SetBridge(this.entities.Create<ShipPart>("ship-part:chassis:mosquito"));
                        ship.Bridge.SetPosition(this.random.NextVector2(-10, 10), this.random.NextSingle(-3, 3));
                    }
                });
            });


            for (Int32 i = 0; i < 20; i++)
            {
                this.entities.Create<ShipPart>("ship-part:triangle").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
                this.entities.Create<ShipPart>("ship-part:square").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
                this.entities.Create<ShipPart>("ship-part:hexagon").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
                this.entities.Create<ShipPart>("ship-part:pentagon").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
                this.entities.Create<ShipPart>("ship-part:thruster:small").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
                this.entities.Create<ShipPart>("ship-part:thruster:small").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
                this.entities.Create<ShipPart>("ship-part:thruster:small").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
                this.entities.Create<ShipPart>("ship-part:thruster:small").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
                this.entities.Create<ShipPart>("ship-part:thruster:small").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));

                this.entities.Create<ShipPart>("ship-part:weapon:mass-driver").SetPosition(this.random.NextVector2(-100, 100), this.random.NextSingle(-3, 3));
            }
        }

        #region Event Handlers
        private void HandleUserJoined(object sender, User arg)
        {
            // Enqueue the new incoming user
            lock(_newUsers)
                _newUsers.Enqueue(arg); 
        }
        #endregion
    }
}
