using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.System.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Library.Contexts;
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
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnChainChanged += this.HandleChainChanged;
            this.OnValidateAmmunitionCollision += this.HandleValidateAmmunitionCollision;
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
        private bool HandleValidateAmmunitionCollision(ShipPart sender, CollisionData data)
        {
            if (data.Fixture.IsSensor)
            {
                // https://stackoverflow.com/questions/31647023/determine-if-angle-is-between-2-other-angles
                var angle = MathHelper.WrapAngle(this.Position.Angle(data.P1));
                var upper = MathHelper.WrapAngle(this.Rotation + (this.Context.Range / 2));
                var lower = MathHelper.WrapAngle(this.Rotation - (this.Context.Range / 2));

                return Math.Abs(MathHelper.WrapAngle(upper - angle)) < MathHelper.PiOver2
                    && Math.Abs(MathHelper.WrapAngle(lower - angle)) < MathHelper.PiOver2;
            }

            return true;
        }

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

                _shield = this.Root.BuildFixture(new CircleShape(10f, 0.0f)
                {
                    Position = Vector2.Transform(Vector2.Zero, this.LocalTransformation)
                }, this);
                _shield.List.ForEach(f => f.IsSensor = true);
            }
        }
        #endregion
    }
}
