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
using VoidHuntersRevived.Library;

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
            
            for (Int32 i=0; i<10; i++)
            {
                var wingLeft = _shipParts.Create("vhr:hull:wing:left");
                wingLeft.Position = rand.NextVector2(0, world.Size.X);
                wingLeft.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var wingRight = _shipParts.Create("vhr:hull:wing:right");
                wingRight.Position = rand.NextVector2(0, world.Size.X);
                wingRight.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var diamond = _shipParts.Create("vhr:hull:diamond");
                diamond.Position = rand.NextVector2(0, world.Size.X);
                diamond.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var triangle = _shipParts.Create("vhr:hull:triangle");
                triangle.Position = rand.NextVector2(0, world.Size.X);
                triangle.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var square = _shipParts.Create("vhr:hull:square");
                square.Position = rand.NextVector2(0, world.Size.X);
                square.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var pentagon = _shipParts.Create("vhr:hull:pentagon");
                pentagon.Position = rand.NextVector2(0, world.Size.X);
                pentagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var hexagon = _shipParts.Create("vhr:hull:hexagon");
                hexagon.Position = rand.NextVector2(0, world.Size.X);
                hexagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var lattice = _shipParts.Create("vhr:hull:lattice");
                lattice.Position = rand.NextVector2(0, world.Size.X);
                lattice.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var vBeam = _shipParts.Create("vhr:hull:beam:vertical");
                vBeam.Position = rand.NextVector2(0, world.Size.X);
                vBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                
                var hBeam = _shipParts.Create("vhr:hull:beam:horizontal");
                hBeam.Position = rand.NextVector2(0, world.Size.X);
                hBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var chevron = _shipParts.Create("vhr:armor:chevron");
                chevron.Position = rand.NextVector2(0, world.Size.X);
                chevron.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var shield = _shipParts.Create("vhr:armor:shield");
                shield.Position = rand.NextVector2(0, world.Size.X);
                shield.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var plate = _shipParts.Create("vhr:armor:plate");
                plate.Position = rand.NextVector2(0, world.Size.X);
                plate.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                var fighterBay = _shipParts.Create("vhr:special:fighter-bay");
                fighterBay.Position = rand.NextVector2(0, world.Size.X);
                fighterBay.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                for (Int32 j = 0; j < 20; j++)
                {
                    var thruster = _shipParts.Create("vhr:thruster:small");
                    thruster.Position = rand.NextVector2(0, world.Size.X);
                    thruster.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                    var thruster2 = _shipParts.Create("vhr:thruster:tiny");
                    thruster2.Position = rand.NextVector2(0, world.Size.X);
                    thruster2.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                    var weapon = _shipParts.Create("vhr:weapon:mass-driver");
                    weapon.Position = rand.NextVector2(0, world.Size.X);
                    weapon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);

                    var weapon2 = _shipParts.Create("vhr:weapon:laser");
                    weapon2.Position = rand.NextVector2(0, world.Size.X);
                    weapon2.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                }
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
                            var rand = new Random();

                            // var chassis = _shipParts.Create("vhr:chassis:fighter");
                            // chassis.Position = rand.NextVector2(0, world.Size.X);
                            // chassis.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                            // ship.Bridge = chassis;

                            var ships = Directory.GetFiles(VHR.Directories.Resources.Ships, "*.vh");
                            using (var fileStream = File.OpenRead(ships[rand.Next(ships.Length)]))
                                ship.Import(fileStream, rand.NextVector2(0, world.Size.X, 0, world.Size.Y));
                        });
                    });
                });
            });
        }
        #endregion
    }
}
