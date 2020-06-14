using FarseerPhysics.Common;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Extensions.Farseer;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public class RigidShipPart : ShipPart
    {
        #region Private Fields
        private Queue<FixtureContainer> _fixtures;
        private Vector2 _localCenter;
        #endregion

        #region Public Attributes
        public override Vector2 Position => this.IsRoot ? base.Position : this.Root.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(this.Root.Rotation));
        public override Single Rotation => this.IsRoot ? base.Rotation : this.Root.Rotation + this.LocalRotation;

        public override Vector2 WorldCenter => this.IsRoot ? base.WorldCenter : this.Root.Position + this.LocalCenter.Rotate(this.Root.Rotation);
        public override Vector2 LocalCenter => _localCenter;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _fixtures = new Queue<FixtureContainer>();

            this.OnRootChanged += this.HandleRootChanged;
        }

        protected override void Dispose()
        {
            base.Dispose();

            while (_fixtures.Any())
                _fixtures.Dequeue().Destroy();

            this.OnRootChanged -= this.HandleRootChanged;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Generate new fixtures onto the inputed body
        /// representing the internal RigidShipPart's defined
        /// fixtures.
        /// </summary>
        /// <param name="root"></param>
        /// <returns>A list of all created fixtures.</returns>
        public void AddFixtures(ShipPart root, Queue<FixtureContainer> fixtures)
        {
            // Auto dispose of any fixtures within the given queue
            while (fixtures.Any())
            {
                this.logger.LogTrace(() => $"Destroying RigidShipPart({this.Id}) Fixture");
                fixtures.Dequeue().Destroy();
            }
                

            // Create new fixtures for all vertices contained in the configuration
            this.Configuration.Vertices.ForEach(data =>
            {
                this.logger.LogTrace(() => $"Creating Fixture for RigidShipPart({this.Id}) on ShipPart({this.Root.Id})");

                Vertices vertices = new Vertices(data);
                vertices.Transform(this.LocalTransformation);
                fixtures.Enqueue(root.BuildFixture(new PolygonShape(vertices, 0.5f), this));
            });
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the root piece changes, we must restructure the entire
        /// ship part to merge with the root.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private void HandleRootChanged(ShipPart sender, ShipPart old, ShipPart value)
        {
            this.AddFixtures(this.Root, _fixtures);

            _localCenter = this.IsRoot ? base.LocalCenter : Vector2.Transform(this.Configuration.Centeroid, this.LocalTransformation);
        }
        #endregion
    }
}
