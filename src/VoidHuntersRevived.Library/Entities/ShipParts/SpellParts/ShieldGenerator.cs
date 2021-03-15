using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.System.Collections;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Services;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Services.Spells;
using VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells;
using VoidHuntersRevived.Library.Utilities.Farseer;

namespace VoidHuntersRevived.Library.Entities.ShipParts.SpellParts
{
    public class ShieldGenerator : ContinuousSpellPart
    {
        #region Private Fields
        private Synchronizer _synchronizer;
        #endregion

        #region Public Properties
        public new ShieldGeneratorContext Context { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _synchronizer);

            this.OnChainChanged += this.HandleChainChanged;
        }

        protected override void Release()
        {
            base.Release();

            _synchronizer = null;

            this.OnChainChanged -= this.HandleChainChanged;

            this.StopCast();
        }
        #endregion

        #region Helper Methods
        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as ShieldGeneratorContext;
        }
        #endregion

        #region SpellPart Implementation
        /// <inheritdoc />
        protected override Spell Cast(GameTime gameTime)
            => this.spells.TryCastEnergyShieldSpell(
                    this.Chain.Ship,
                    0f, // This is a 0 mana spell
                    this);
        #endregion

        #region Event Handlers
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
            => this.StopCast();
        #endregion
    }
}
