using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Lists;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts.Special
{
    internal sealed class FighterBayMasterAuthorizationDriver : MasterNetworkAuthorizationDriver<FighterBay>
    {
        #region Private Fields
        private Synchronizer _synchronizer;
        private EntityList _entities;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(FighterBay driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _synchronizer);
            provider.Service(out _entities);

            this.driven.OnLaunch += this.HandleLaunch;
        }

        protected override void Release(FighterBay driven)
        {
            base.Release(driven);

            this.driven.OnLaunch -= this.HandleLaunch;
        }
        #endregion

        #region Event Handlers
        private void HandleLaunch(FighterBay sender)
        {
            _synchronizer.Enqueue(gt =>
            {
                _entities.Create<ComputerPlayer>((player, p, d) =>
                {
                    player.Ship = _entities.Create<Ship>((ship, p2, c) =>
                    {
                        player.Team = this.driven.Root.Chain.Ship.Player.Team;

                        using (var fileStream = File.OpenRead("Resources/Ships/fighter.vh"))
                            ship.Import(fileStream, this.driven.Position, this.driven.Rotation + MathHelper.Pi);

                        ship.Bridge.ApplyForce(new Vector2(200, 0).RotateTo(this.driven.Rotation + MathHelper.Pi), this.driven.Position);

                        ship.Bridge.CollidesWith = VHR.Categories.PassiveCollidesWith;
                        ship.Bridge.CollisionCategories = VHR.Categories.PassiveCollisionCategories;
                    });
                });
            });
        }
        #endregion
    }
}
