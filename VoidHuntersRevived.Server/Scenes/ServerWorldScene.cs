﻿using System;
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
using System.IO;
using VoidHuntersRevived.Library.Utilities;
using System.Linq;
using VoidHuntersRevived.Library.Extensions.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerWorldScene : WorldScene
    {
        #region Private Fields
        private List<Team> _teams;
        private Queue<User> _newUsers = new Queue<User>();
        private List<Player> _players;
        #endregion

        #region Constructor
        public ServerWorldScene(List<Team> teams)
        {
            _teams = teams;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _players = provider.GetRequiredService<List<Player>>();
        }

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

            var rand = new Random();
            var size = 250;

            this.entities.Create<Team>("entity:team", t =>
            {
                t.Color = new Color(1, 203, 226);
            });
            this.entities.Create<Team>("entity:team", t =>
            {
                t.Color = new Color(195, 199, 43);
            });
            this.entities.Create<Team>("entity:team", t =>
            {
                t.Color = new Color(2, 224, 73);
            });
            this.entities.Create<Team>("entity:team", t =>
            {
                t.Color = new Color(224, 2, 187);
            });
            this.entities.Create<Team>("entity:team", t =>
            {
                t.Color = new Color(245, 245, 245);
            });

            for (Int32 i=0; i<200; i++)
            {
                this.entities.Create<ShipPart>("entity:ship-part:hull:triangle", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:hull:square", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:hull:pentagon", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:hull:hexagon", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:thruster:small", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:weapon:mass-driver", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
            }

            for (Int32 i = 0; i < 0; i++)
            {
                this.entities.Create<ComputerPlayer>("entity:player:computer", p =>
                {
                    p.SetShip(this.entities.Create<Ship>("entity:ship", s =>
                    {
                        using (FileStream input = File.OpenRead("Ships/mosquito.vh"))
                            s.Import(input);

                        s.Bridge.Body.SetTransformIgnoreContacts(rand.NextVector2(-100, 100), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                    }));
                });
            }
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            this.Group.Users.OnAdded += this.HandleUserJoined;
            this.Group.Users.OnRemoved += this.HandleUserLeft;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.Group.Users.OnAdded -= this.HandleUserJoined;
            this.Group.Users.OnAdded -= this.HandleUserLeft;
        }
        #endregion

        #region Frame Methods 
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_newUsers.Any())
            {
                this.entities.Create<UserPlayer>("entity:player:user", p =>
                {
                    (new Random()).Next(_teams).AddPlayer(p);
                    p.User = _newUsers.Dequeue();
                    p.SetShip(this.entities.Create<Ship>("entity:ship", s =>
                    {
                        using (FileStream input = File.OpenRead("Ships/mosquito.vh"))
                            s.Import(input);
                        // s.SetBridge(this.entities.Create<ShipPart>("entity:ship-part:chassis:mosquito"));

                    var rand = new Random();
                        s.Bridge.Body.SetTransformIgnoreContacts(rand.NextVector2(-15, 15), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                    }));
                });
            }
        }
        #endregion

        #region Event Handlers
        private void HandleUserJoined(object sender, User arg)
        {
            this.logger.LogInformation($"{arg.Name} has connected.");

            if (_players.FirstOrDefault(p => p is UserPlayer && (p as UserPlayer).User == arg) == default(Player))
                _newUsers.Enqueue(arg);
        }

        private void HandleUserLeft(object sender, User arg)
        {
            this.logger.LogInformation($"{arg.Name} has disconnected.");
            foreach(UserPlayer player in _players.Where(p => p is UserPlayer && (p as UserPlayer).User == arg).Select(p => p as UserPlayer).ToList()) {
                player.Dispose();
            }
        }
        #endregion
    }
}
