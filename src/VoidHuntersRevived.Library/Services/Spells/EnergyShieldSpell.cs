using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.System.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells;
using VoidHuntersRevived.Library.Utilities.Farseer;

namespace VoidHuntersRevived.Library.Services.Spells
{
    /// <summary>
    /// The primary spell responsible for blocking
    /// weapons. This contains the sensor Fixture
    /// for AmmunitionSpell collision detection.
    /// </summary>
    public class EnergyShieldSpell : Spell, IAmmunitionSpellTarget
    {
        #region Private Fields
        private FixtureContainer _fixture;
        #endregion

        #region Public Properties
        public ShieldGenerator ShieldGenerator { get; internal set; }
        #endregion

        #region Events
        public event ValidateEventDelegate<IAmmunitionSpellTarget, AmmunitionSpell.CollisionData> OnValidateAmmunitionCollision;
        public event IAmmunitionSpellTarget.ApplyAmmunitionCollisionDelegate OnApplyAmmunitionCollision;
        #endregion

        #region Lifecycle Methods

        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnValidateAmmunitionCollision += this.HandleValidateAmmunitionCollision;
            this.OnApplyAmmunitionCollision += this.HandleApplyAmmunitionCollision;
        }

        protected override void Release()
        {
            base.Release();

            _fixture.Destroy();
            _fixture = null;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnValidateAmmunitionCollision -= this.HandleValidateAmmunitionCollision;
            this.OnApplyAmmunitionCollision -= this.HandleApplyAmmunitionCollision;
        }
        #endregion

        #region Helper Methods
        protected override void Invoke(Spell spell)
        {
            base.Invoke(spell);

            _fixture = this.ShieldGenerator.Root.BuildFixture(new CircleShape(this.ShieldGenerator.Context.Radius, 0.0f)
            {
                Position = Vector2.Transform(Vector2.Zero, this.ShieldGenerator.LocalTransformation)
            }, this);

            _fixture.List.ForEach(f => f.IsSensor = true);
        }
        #endregion

        #region IAmmunitionSpellTarget Implementation 
        void IAmmunitionSpellTarget.ApplyAmmunitionCollision(AmmunitionSpell.CollisionData data, GameTime gameTime)
            => this.OnApplyAmmunitionCollision?.Invoke(this, data, gameTime);


        bool IAmmunitionSpellTarget.ValidateAmmunitionCollision(AmmunitionSpell.CollisionData data)
            => this.OnValidateAmmunitionCollision.Validate(this, data, false);
        #endregion

        #region Event Handlers
        private bool HandleValidateAmmunitionCollision(IAmmunitionSpellTarget sender, AmmunitionSpell.CollisionData collision)
        {
            // https://stackoverflow.com/questions/31647023/determine-if-angle-is-between-2-other-angles
            var angle = MathHelper.WrapAngle(this.ShieldGenerator.Position.Angle(collision.P1));
            var upper = MathHelper.WrapAngle(this.ShieldGenerator.Rotation + (this.ShieldGenerator.Context.Range / 2));
            var lower = MathHelper.WrapAngle(this.ShieldGenerator.Rotation - (this.ShieldGenerator.Context.Range / 2));

            var withinShieldBounds = Math.Abs(MathHelper.WrapAngle(upper - angle)) < MathHelper.PiOver2
                && Math.Abs(MathHelper.WrapAngle(lower - angle)) < MathHelper.PiOver2;

            // Determin whether or not the shield was actually hit...
            if (withinShieldBounds)
                return true;
            else
                return false;
        }

        private void HandleApplyAmmunitionCollision(IAmmunitionSpellTarget sender, AmmunitionSpell.CollisionData data, GameTime gameTime)
        {
            this.ShieldGenerator.Chain.Ship.TryConsumeMana(data.Ammunition.GetShieldDeflectionManaCost(data, gameTime));
        }
        #endregion
    }
}
