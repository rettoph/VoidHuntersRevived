using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.System.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities.Farseer;
using static VoidHuntersRevived.Library.Entities.Ammunitions.Ammunition;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Special
{
    public class ShieldGenerator : RigidShipPart
    {
        #region Private Fields
        private FixtureContainer _shield;
        #endregion

        #region Public Properties
        public new ShieldGeneratorContext Context { get; private set; }

        /// <summary>
        /// Determins whether or not the current shield is
        /// actually on & powered. Note that just because
        /// a shield is active doesnt mean its powered.
        /// </summary>
        public Boolean Powered => this.Active && (!this.Chain.Ship?.Charging ?? false) && this.Health > 0;

        /// <summary>
        /// Determins that the current shield would be on
        /// if possible.
        /// </summary>
        public Boolean Active { get; private set; } = true;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnChainChanged += this.HandleChainChanged;
            this.OnValidateAmmunitionCollision += this.HandleValidateAmmunitionCollision;
            this.OnApplyAmmunitionCollision += this.HandleApplyAmmunitionCollision;
            this.OnValidateAmmunitionCollisionDamage += this.HandleApplyAmmunitionCollisionDamage;
        }

        protected override void Release()
        {
            base.Release();

            _shield?.Destroy();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnChainChanged -= this.HandleChainChanged;
            this.OnValidateAmmunitionCollision -= this.HandleValidateAmmunitionCollision;
            this.OnApplyAmmunitionCollision -= this.HandleApplyAmmunitionCollision;
            this.OnValidateAmmunitionCollisionDamage -= this.HandleApplyAmmunitionCollisionDamage;
        }
        #endregion

        #region Helper Methods
        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as ShieldGeneratorContext;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the chain changes, we must restructure the entire
        /// ship part to merge with the root.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if (value != default)
            {
                _shield?.Destroy();

                _shield = this.Root.BuildFixture(new CircleShape(this.Context.Radius, 0.0f)
                {
                    Position = Vector2.Transform(Vector2.Zero, this.LocalTransformation)
                }, this);
                _shield.List.ForEach(f => f.IsSensor = true);
            }
        }

        /// <summary>
        /// Check to see if the ammunition is hitting the energy shield.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool HandleValidateAmmunitionCollision(ShipPart sender, CollisionData collision)
        {
            if (_shield.List.Contains(collision.Fixture))
            {
                // Guard clauses for instant no.
                if (!this.Powered)
                    return false;

                // https://stackoverflow.com/questions/31647023/determine-if-angle-is-between-2-other-angles
                var angle = MathHelper.WrapAngle(this.Position.Angle(collision.P1));
                var upper = MathHelper.WrapAngle(this.Rotation + (this.Context.Range / 2));
                var lower = MathHelper.WrapAngle(this.Rotation - (this.Context.Range / 2));

                var withinShieldBounds = Math.Abs(MathHelper.WrapAngle(upper - angle)) < MathHelper.PiOver2
                    && Math.Abs(MathHelper.WrapAngle(lower - angle)) < MathHelper.PiOver2;

                // Determin whether or not the shield was actually hit...
                if (withinShieldBounds)
                    return true;
                else
                    return false;
            }

            return true;
        }

        private void HandleApplyAmmunitionCollision(ShipPart sender, CollisionData data, GameTime gameTime)
        {
            if(data.Fixture.IsSensor)
                this.Chain.Ship.TryUseEnergy(
                    data.Ammunition.GetShieldDeflectionEnergyCost(
                        data,
                        gameTime));
        }

        private bool HandleApplyAmmunitionCollisionDamage(ShipPart sender, CollisionData data)
            => !data.Fixture.IsSensor;
        #endregion
    }
}
