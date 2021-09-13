using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Library.Dtos.Utilities;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// A <see cref="ShipPart"/> that will automatically create a
    /// Fixture its current <see cref="Chain"/>'s Body.
    /// </summary>
    public class RigidShipPart : ShipPart
    {
        #region Private Fields
        private Queue<AetherFixture> _fixtures;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            _fixtures = new Queue<AetherFixture>();
        }
        #endregion


        #region Helper Methods
        protected override void TryCreateCorporealForm(Chain chain)
        {
            base.TryCreateCorporealForm(chain);

            foreach (Shape shape in this.Context.Shapes)
            {
                _fixtures.Enqueue(
                    chain.Body.CreateFixture(
                        shape.Clone(this.LocalTransformation),
                        this));
            }
        }

        protected override void TryDestroyCorporealForm()
        {
            base.TryDestroyCorporealForm();

            while (_fixtures.Any())
            {
                _fixtures.Dequeue().TryRelease();
            }
        }
        #endregion
    }
}
