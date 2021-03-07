using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Extensions.Services;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Special
{
    public class DroneBay : SpellCasterPart
    {
        #region Public Properties
        public new DroneBayContext Context { get; private set; }
        #endregion

        #region API Methods
        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as DroneBayContext;
        }

        protected override void Cast(GameTime gameTime)
            => this.spells.CastLaunchDrone(
                    position: this.Position,
                    rotation: this.Rotation + MathHelper.Pi,
                    maxAge: this.Context.DroneMaxAge,
                    type: this.Context.DroneType,
                    team: this.Root.Chain.Ship.Player.Team);
        #endregion
    }
}
