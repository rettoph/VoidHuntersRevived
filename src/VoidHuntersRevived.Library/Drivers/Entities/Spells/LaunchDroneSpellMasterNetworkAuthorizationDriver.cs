using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Lists;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Spells;

namespace VoidHuntersRevived.Library.Drivers.Entities.Spells
{
    public class LaunchDroneSpellMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<LaunchDroneSpell>
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(LaunchDroneSpell driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _entities);

            this.driven.OnCast += this.Cast;
        }

        protected override void Release(LaunchDroneSpell driven)
        {
            base.Release(driven);

            _entities = null;

            this.driven.OnCast -= this.Cast;
        }
        #endregion

        #region Methods
        private void Cast(Spell sender)
        {
            _entities.Create<DronePlayer>((player, p, d) =>
            {
                player.Ship = _entities.Create<Ship>((ship, p2, c) =>
                {
                    player.MaxAge = this.driven.MaxAge;
                    player.Team = this.driven.Team;

                    using (var fileStream = File.OpenRead($"{VHR.Directories.Resources.Ships}/{this.driven.Type}.vh"))
                        ship.Import(fileStream, this.driven.Position, this.driven.Rotation);

                    ship.Bridge.ApplyForce(new Vector2(200, 0).RotateTo(this.driven.Rotation), this.driven.Position);

                    ship.Bridge.CollidesWith = VHR.Categories.PassiveCollidesWith;
                    ship.Bridge.CollisionCategories = VHR.Categories.PassiveCollisionCategories;
                });
            });

            this.driven.TryRelease();
        }
        #endregion
    }
}
