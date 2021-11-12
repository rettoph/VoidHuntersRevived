using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Dtos.Utilities;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// A <see cref="ShipPart"/> that will automatically create a
    /// Fixture its current <see cref="Chain"/>'s Body.
    /// </summary>
    public class RigidShipPart<TShipPartContext> : ShipPart<TShipPartContext>
        where TShipPartContext : ShipPartContext
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
        protected override void TryCreateAetherForm(Chain chain)
        {
            base.TryCreateAetherForm(chain);

            foreach (ShapeDto shape in this.Context.Shapes)
            {
                if(shape.IsCorporeal)
                {
                    _fixtures.Enqueue(
                    chain.Body.CreateFixture(
                        shape.Data.Clone(this.LocalTransformation),
                        this));
                }
            }
        }

        protected override void TryDestroyAetherForm()
        {
            base.TryDestroyAetherForm();

            while (_fixtures.Any())
            {
                _fixtures.Dequeue().TryRelease();
            }
        }

        protected override void TryUpdateCorporealState(bool corporeal)
        {
            base.TryUpdateCorporealState(corporeal);

            if(corporeal)
            {
                foreach(AetherFixture fixture in _fixtures)
                {
                    fixture.SetCollisionData(
                        collidesWith: Constants.CollisionCategories.CorporealCollidesWith,
                        collisionCategories: Constants.CollisionCategories.CorporealCollisionCategories,
                        collisionGroup: Constants.CollisionCategories.CorporealCollisionGroup);
                }
            }
            else
            {
                foreach (AetherFixture fixture in _fixtures)
                {
                    fixture.SetCollisionData(
                        collidesWith: Constants.CollisionCategories.NonCorporealCollidesWith,
                        collisionCategories: Constants.CollisionCategories.NonCorporealCollisionCategories,
                        collisionGroup: Constants.CollisionCategories.NonCorporealCollisionGroup);
                }
            }
        }

        public override string ToAetherString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Local Position: {Vector2.Transform(Vector2.Zero, this.LocalTransformation)}");

            foreach(AetherFixture afixture in _fixtures)
            {
                foreach(Fixture fixture in afixture.Instances.Values)
                {
                    if(fixture.Tag is ShipPart shipPart)
                    {
                        sb.AppendLine("Tag.Id: " + shipPart.Id);
                    }
                    
                }
            }

            return sb.ToString();
        }
        #endregion
    }
}
