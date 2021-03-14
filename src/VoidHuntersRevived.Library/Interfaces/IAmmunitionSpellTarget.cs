using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services.Spells;
using tainicom.Aether.Physics2D.Dynamics;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells;

namespace VoidHuntersRevived.Library.Interfaces
{
    /// <summary>
    /// The primary interface responsible for detecting a collision
    /// with a <see cref="AmmunitionSpell"/>. An instance of this interface
    /// must be the <see cref="Fixture.Tag"/> value.
    /// </summary>
    public interface IAmmunitionSpellTarget
    {
        #region Events
        public delegate void ApplyAmmunitionCollisionDelegate(IAmmunitionSpellTarget sender, AmmunitionSpell.CollisionData data, GameTime gameTime);

        /// <summary>
        /// Determin whether or not a collision should be applied on the selected
        /// <see cref="IAmmunitionSpellTarget"/>.
        /// </summary>
        public event ValidateEventDelegate<IAmmunitionSpellTarget, AmmunitionSpell.CollisionData> OnValidateAmmunitionCollision;

        /// <summary>
        /// Apply the ammunition on the current <see cref="IAmmunitionSpellTarget"/>.
        /// </summary>
        public event ApplyAmmunitionCollisionDelegate OnApplyAmmunitionCollision;
        #endregion

        #region Events
        /// <summary>
        /// Invoke the <see cref="IAmmunitionSpellTarget.OnValidateAmmunitionCollision"/> event & 
        /// return its response.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Boolean ValidateAmmunitionCollision(AmmunitionSpell.CollisionData data);

        /// <summary>
        /// Invoke the <see cref="IAmmunitionSpellTarget.OnApplyAmmunitionCollision"/> event.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="gameTime"></param>
        void ApplyAmmunitionCollision(AmmunitionSpell.CollisionData data, GameTime gameTime);
        #endregion
    }
}
