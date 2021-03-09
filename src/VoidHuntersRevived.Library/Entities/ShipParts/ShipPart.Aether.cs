using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Utilities.Farseer;
using VoidHuntersRevived.Library.Extensions.Aether;
using Guppy.Extensions.Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Partial class responsible for maintaining the Aether specific
    /// funcitonality of a ship part. By default a Rigid connection
    /// is created but may be overwritten.
    /// </summary>
    public partial class ShipPart
    {
        #region Private Fields
        private Queue<FixtureContainer> _internalFixtures;
        private Vector2 _localCenter;
        #endregion

        #region Protected Properties
        /// <summary>
        /// an internal collection of fixtures
        /// that belong to the current ship part.
        /// </summary>
        protected IReadOnlyCollection<FixtureContainer> internalFixtures => _internalFixtures;
        #endregion

        #region Public Attributes
        public override Vector2 Position => this.IsRoot ? base.Position : this.Root.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(this.Root.Rotation));
        public override Single Rotation => this.IsRoot ? base.Rotation : this.Root.Rotation + this.LocalRotation;

        public override Vector2 WorldCenter => this.IsRoot ? base.WorldCenter : this.Root.Position + this.LocalCenter.Rotate(this.Root.Rotation);
        public override Vector2 LocalCenter => _localCenter;
        #endregion

        #region Lifecycle Methods
        private void Aether_Create(ServiceProvider provider)
        {
            _internalFixtures = new Queue<FixtureContainer>();

            this.OnChainChanged += ShipPart.Aether_HandleChainChanged;
        }

        private void Aether_Release()
        {
            while (_internalFixtures.Any())
                _internalFixtures.Dequeue().Destroy();
        }

        private void Aether_Dispose()
        {
            this.OnChainChanged -= ShipPart.Aether_HandleChainChanged;
        }
        #endregion

        #region Helper Methods
        private void UpdateInternalFixtures()
        {
            // Auto dispose of any fixtures within the given queue
            while (_internalFixtures.Any())
                _internalFixtures.Dequeue().Destroy();

            this.BuildInternalFixtures(_internalFixtures);

            _localCenter = this.IsRoot ? base.LocalCenter : Vector2.Transform(this.Context.Centeroid, this.LocalTransformation);
        }
        /// <summary>
        /// Generate new fixtures onto the inputed body
        /// representing the internal RigidShipPart's defined
        /// fixtures.
        /// </summary>
        /// <param name="fixtures">a queue of fixtures that all new instances should be added into.</param>
        /// <returns>A list of all created fixtures.</returns>
        protected virtual void BuildInternalFixtures(Queue<FixtureContainer> fixtures)
        {

            // Create new fixtures for all vertices contained in the configuration
            foreach (ShapeContext shape in this.Context.InnerShapes)
            {
                if (shape.Solid)
                {
                    Vertices vertices = new Vertices(shape.Vertices);
                    vertices.Transform(this.LocalTransformation);
                    fixtures.Enqueue(this.Root.BuildFixture(new PolygonShape(vertices, this.Context.Density), this));
                }
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the chain changes, we must restructure the entire
        /// ship part to merge with the root.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="old"></param>
        /// <param name="value"></param>
        private static void Aether_HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if (value != default)
            {
                sender.UpdateInternalFixtures();
            }
        }
        #endregion
    }
}
