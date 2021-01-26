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
using Guppy.Extensions.log4net;
using Guppy.Lists.Interfaces;
using Guppy.Network;
using VoidHuntersRevived.Library.Entities.Players;
using System.IO;
using Guppy.Utilities;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Server.Scenes
{
    public sealed class ServerGameScene : GameScene
    {
        #region Private Fields
        private Synchronizer _synchronizer;
        private ShipPartService _shipParts;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _synchronizer);
            provider.Service(out _shipParts);

            this.group.Users.OnAdded += this.HandleUserJoined;
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);


            this.logger.Debug(() => "Server");

            var world = this.Entities.Create<WorldEntity>((w, p, c) =>
            {
                w.Size = new Vector2(Chunk.Size * 5, Chunk.Size * 5);
            });

            var rand = new Random(1);
            
            for(Int32 i=0; i < 0; i++)
            {
                this.Entities.Create<ComputerPlayer>((player, p, d) =>
                {
                    player.Ship = this.Entities.Create<Ship>((ship, p2, c) =>
                    {
                        ship.Import(File.OpenRead("Resources/Ships/mosquito.vh"), rand.NextVector2(0, world.Size.X, 0, world.Size.Y));
                    });
                });
            }
            
            for (Int32 i=0; i<50; i++)
            {
                var armorShield = _shipParts.Create("vhr:armor:demo");
                armorShield.Position = rand.NextVector2(0, world.Size.X);
                armorShield.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // var armorPlate = _shipParts.Create("armor:plate");
                // armorPlate.Position = rand.NextVector2(0, world.Size.X);
                // armorPlate.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // var triangle = _shipParts.Create("hull:triangle");
                // triangle.Position = rand.NextVector2(0, world.Size.X);
                // triangle.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                var square = _shipParts.Create("vhr:hull:square");
                square.Position = rand.NextVector2(0, world.Size.X);
                square.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // var hexagon = _shipParts.Create("hull:hexagon");
                // hexagon.Position = rand.NextVector2(0, world.Size.X);
                // hexagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // var diamond = _shipParts.Create("hull:diamond");
                // diamond.Position = rand.NextVector2(0, world.Size.X);
                // diamond.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // var pentagon = _shipParts.Create("hull:pentagon");
                // pentagon.Position = rand.NextVector2(0, world.Size.X);
                // pentagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                var vBeam = _shipParts.Create("vhr:hull:beam:vertical");
                vBeam.Position = rand.NextVector2(0, world.Size.X);
                vBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                
                var hBeam = _shipParts.Create("vhr:hull:beam:horizontal");
                hBeam.Position = rand.NextVector2(0, world.Size.X);
                hBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // var thruster = _shipParts.Create("thruster:small");
                // thruster.Position = rand.NextVector2(0, world.Size.X);
                // thruster.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // var weapon = _shipParts.Create("weapon:gun:mass-driver");
                // weapon.Position = rand.NextVector2(0, world.Size.X);
                // weapon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                // 
                // var hexagon = this.Entities.Create<ShipPart>("entity:ship-part:hull:hexagon");
                // hexagon.Position = rand.NextVector2(0, world.Size.X);
                // hexagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // var pentagon = this.Entities.Create<ShipPart>("entity:ship-part:hull:pentagon");
                // pentagon.Position = rand.NextVector2(0, world.Size.X);
                // pentagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                // if (i % 2 == 0)
                // {
                //     var vBeam = this.Entities.Create<ShipPart>("entity:ship-part:hull:beam:vertical");
                //     vBeam.Position = rand.NextVector2(0, world.Size.X);
                //     vBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // }
                // else
                // {
                //     var hBeam = this.Entities.Create<ShipPart>("entity:ship-part:hull:beam:horizontal");
                //     hBeam.Position = rand.NextVector2(0, world.Size.X);
                //     hBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // }
                // 
                // for (Int32 j = 0; j < 10; j++)
                // {
                //     var thruster = this.Entities.Create<ShipPart>("entity:ship-part:weapon:mass-driver");
                //     thruster.Position = rand.NextVector2(0, world.Size.X);
                //     thruster.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // 
                //     var weapon = this.Entities.Create<ShipPart>("entity:ship-part:thruster:small");
                //     weapon.Position = rand.NextVector2(0, world.Size.X);
                //     weapon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                // }
            }
        }

        protected override void Release()
        {
            base.Release();

            _synchronizer = null;
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
            this.IfOrOnWorld(world =>
            {
                _synchronizer.Enqueue(gt =>
                {
                    this.Entities.Create<UserPlayer>((player, p, d) =>
                    {
                        player.User = user;
                        player.Ship = this.Entities.Create<Ship>((ship, p2, c) =>
                        {
                            // var ships = Directory.GetFiles("Resources/Ships", "*.vh");
                            // var rand = new Random();
                            // using (var fileStream = File.OpenRead(ships[rand.Next(ships.Length)]))
                            //     ship.Import(fileStream, rand.NextVector2(0, world.Size.X, 0, world.Size.Y));

                            ship.Bridge = _shipParts.Create("vhr:hull:square");
                        });
                    });
                });
            });

        }
        #endregion
    }
}
