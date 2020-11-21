using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.System;
using VoidHuntersRevived.Library.Scenes;
using Guppy.IO.Extensions.log4net;
using Guppy.Lists.Interfaces;
using Guppy.Network;
using VoidHuntersRevived.Library.Entities.Players;
using System.IO;
using Guppy.Utilities;

namespace VoidHuntersRevived.Server.Scenes
{
    public sealed class ServerGameScene : GameScene
    {
        #region Private Fields
        private Synchronizer _synchronizer;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _synchronizer);

            this.group.Users.OnAdded += this.HandleUserJoined;
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);


            this.log.Debug(() => "Server");

            var world = this.Entities.Create<WorldEntity>((w, p, c) =>
            {
                w.Size = new Vector2(Chunk.Size * 5, Chunk.Size * 5);
            });

            var rand = new Random(1);
            for(Int32 i=0; i<1; i++)
            {
                var triangle = this.Entities.Create<ShipPart>("entity:ship-part:hull:triangle");
                triangle.Position = rand.NextVector2(0, world.Size.X);
                triangle.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                var square = this.Entities.Create<ShipPart>("entity:ship-part:hull:square");
                square.Position = rand.NextVector2(0, world.Size.X);
                square.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                var hexagon = this.Entities.Create<ShipPart>("entity:ship-part:hull:hexagon");
                hexagon.Position = rand.NextVector2(0, world.Size.X);
                hexagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                var pentagon = this.Entities.Create<ShipPart>("entity:ship-part:hull:pentagon");
                pentagon.Position = rand.NextVector2(0, world.Size.X);
                pentagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                if (i % 2 == 0)
                {
                    var vBeam = this.Entities.Create<ShipPart>("entity:ship-part:hull:beam:vertical");
                    vBeam.Position = rand.NextVector2(0, world.Size.X);
                    vBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                }
                else
                {
                    var hBeam = this.Entities.Create<ShipPart>("entity:ship-part:hull:beam:horizontal");
                    hBeam.Position = rand.NextVector2(0, world.Size.X);
                    hBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                }
            
                for (Int32 j = 0; j < 20; j++)
                {
                    var thruster = this.Entities.Create<ShipPart>("entity:ship-part:thruster:small");
                    thruster.Position = rand.NextVector2(0, world.Size.X);
                    thruster.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                    var weapon = this.Entities.Create<ShipPart>("entity:ship-part:thruster:small");
                    weapon.Position = rand.NextVector2(0, world.Size.X);
                    weapon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                }
            }
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When a new user joins we should create a brand new 
        /// player instance for them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleUserJoined(IServiceList<User> sender, User user)
        {
            _synchronizer.Enqueue(gt =>
            {
                this.Entities.Create<UserPlayer>((player, p, d) =>
                {
                    player.User = user;
                    player.Ship = this.Entities.Create<Ship>((ship, p2, c) =>
                    {
                        ship.Import(File.OpenRead("Ships/mosquito.vh"));
                        
                        // ship.SetBridge(this.Entities.Create<ShipPart>("entity:ship-part:chassis:mosquito"));
                        ship.Bridge.Position = (new Random()).NextVector2(0, 20);
                    });
                });
            });
        }
        #endregion
    }
}
