using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

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
        protected override Ammunition Fire(ServiceProvider provider)
        {
            return provider.GetService<Bullet>((b, p, d) =>
            {
                b.Position = this.Position;
                b.Velocity = Vector2.Transform(Vector2.UnitX * 2, Matrix.CreateRotationZ(MathHelper.WrapAngle(this.Rotation + MathHelper.Pi)));
            });
        }
        #endregion
    }
}
