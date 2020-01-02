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
using System.IO;
using VoidHuntersRevived.Library.Utilities;
using System.Linq;
using VoidHuntersRevived.Library.Extensions.Collections;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerWorldScene : WorldScene
    {
        #region Private Fields
        private ShipBuilder _shipBuilder;
        private List<Team> _teams;
        private Queue<User> _newUsers = new Queue<User>();
        #endregion

        #region Constructor
        public ServerWorldScene(List<Team> teams, ShipBuilder shipBuilder)
        {
            _teams = teams;
            _shipBuilder = shipBuilder;
        }
        #endregion

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

            this.Group.Users.OnAdded += this.HandleUserJoined;

            var rand = new Random();
            var size = 100;

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

            for (Int32 i=0; i<20; i++)
            {
                this.entities.Create<ShipPart>("entity:ship-part:hull:triangle", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
                this.entities.Create<ShipPart>("entity:ship-part:hull:square", e => e.Body.SetTransformIgnoreContacts(rand.NextVector2(-size, size), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi)));
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
                            s.SetBridge(_shipBuilder.Import(input));

                        s.Bridge.Body.SetTransformIgnoreContacts(rand.NextVector2(-100, 100), rand.NextSingle(-MathHelper.Pi, MathHelper.Pi));
                    }));
                });
            }
        }
        #endregion

        #region Frame Methods 
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_newUsers.Any())
            {
                this.entities.Create<UserPlayer>("entity:player:user", p =>
                {
                    (new Random()).Next(_teams).AddPlayer(p);
                    p.User = _newUsers.Dequeue();
                    p.SetShip(this.entities.Create<Ship>("entity:ship", s =>
                    {
                        using (FileStream input = File.OpenRead("Ships/mosquito.vh"))
                            s.SetBridge(_shipBuilder.Import(input));
                    //s.SetBridge(this.entities.Create<ShipPart>("entity:ship-part:hull:square"));

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
            _newUsers.Enqueue(arg);
        }
        #endregion
    }
}
