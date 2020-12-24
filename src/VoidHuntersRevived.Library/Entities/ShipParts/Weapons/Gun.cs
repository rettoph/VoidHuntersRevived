using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Guppy.Lists;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// Simple weapon implementation that will fire
    /// projectiles
    /// </summary>
    public class Gun : Weapon
    {
        #region Weapon Implementation
        /// <inheritdoc />
        protected override Ammunition Fire(ServiceProvider provider, EntityList entities)
        {
            return entities.Create<Bullet>((b, p, d) =>
            {
                b.Damage = 100f;
                b.Position = this.Position;
                b.Velocity = this.Root.LinearVelocity + Vector2.Transform(Vector2.UnitX * 15, Matrix.CreateRotationZ(MathHelper.WrapAngle(this.Rotation + MathHelper.Pi)));
            });
        }
        #endregion
    }
}
