using Guppy.DependencyInjection;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.Ammunitions;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    public class Laser : Weapon
    {
        #region Public Properties
        public new LaserContext Context { get; private set; }
        #endregion

        #region Helper Methods
        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as LaserContext;
        }
        #endregion

        #region Weapon Implementation
        /// <inheritdoc />
        protected override Ammunition Fire(ServiceProvider provider, EntityList entities, Action<Ammunition, ServiceProvider, ServiceConfiguration> setup)
        {
            return entities.Create<LaserBeam>((lb, p, d) =>
            {
                setup(lb, p, d);

                lb.DamagePerSecond = this.Context.DamagePerSecond;
                lb.ShieldDeflectionEnergyCostPerSecond = this.Context.ShieldDeflectionEnergyCostPerSecond;
            });
        }
        #endregion
    }
}
