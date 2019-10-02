using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Utilities;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// Implementation of the ShipPart class that creates a rigig attachment
    /// via fixture transplanting.
    /// </summary>
    public class RigidShipPart : ShipPart
    {
        #region Private Fields
        private Queue<Fixture> _fixtures = new Queue<Fixture>();
        #endregion

        /// <summary>
        /// Destroy all old fixtures and re-create them transplaneted onto
        /// the root piece
        /// </summary>
        protected override void UpdateChainPlacement()
        {
            Fixture target;

            while (_fixtures.Any()) // Destroy all self contained fixtures
                ((target = _fixtures.Dequeue()).Body.UserData as ShipPart).DestroyFixture(target);

            // Create new fixtures for all vertices contained in the configuration
            this.config.Vertices.ForEach(data =>
            {
                Vertices vertices = new Vertices(data);
                Matrix _currentChainTranslation = this.LocalTransformation;
                vertices.Transform(ref _currentChainTranslation);
                _fixtures.Enqueue(this.Root.CreateFixture(new PolygonShape(vertices, 0.5f), this));
            });
        }

        /// <summary>
        /// When creating a new fixture, update the collision categories and collides with values.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="userData"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public override Fixture CreateFixture(Shape shape, object userData = null, Action<Fixture> setup = null)
        {
            return base.CreateFixture(shape, userData, f =>
            {
                if (this.Root.IsBridge)
                {
                    f.CollidesWith = CollisionCategories.ActiveCollidesWith;
                    f.CollisionCategories = CollisionCategories.ActiveCollisionCategories;
                }
                else
                {
                    f.CollidesWith = CollisionCategories.PassiveCollidesWith;
                    f.CollisionCategories = CollisionCategories.PassiveCollisionCategories;
                }

                setup?.Invoke(f);
            });
        }
    }
}
