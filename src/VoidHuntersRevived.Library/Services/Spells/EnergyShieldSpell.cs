using Guppy.Events.Delegates;
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
        protected override void Release()
        {
            base.Release();

            _fixture.Destroy();
            _fixture = null;
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
        }
        #endregion

        #region IAmmunitionSpellTarget Implementation 
        void IAmmunitionSpellTarget.ApplyAmmunitionCollision(AmmunitionSpell.CollisionData data, GameTime gameTime)
            => this.OnApplyAmmunitionCollision.Invoke(this, data, gameTime);


        bool IAmmunitionSpellTarget.ValidateAmmunitionCollision(AmmunitionSpell.CollisionData data)
            => this.OnValidateAmmunitionCollision.Validate(this, data, false);
        #endregion
    }
}
