using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using VoidHuntersRevived.Library.Extensions.Aether;
using Microsoft.Xna.Framework;
using Guppy.Extensions.log4net;
using VoidHuntersRevived.Library.Utilities.Farseer;
using Guppy.Extensions.Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public class RigidShipPart : ShipPart
    {
        #region Private Fields
        private Queue<FixtureContainer> _rootFixtures;
        private Vector2 _localCenter;
        #endregion

        #region Public Attributes
        public override Vector2 Position => this.IsRoot ? base.Position : this.Root.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(this.Root.Rotation));
        public override Single Rotation => this.IsRoot ? base.Rotation : this.Root.Rotation + this.LocalRotation;

        public override Vector2 WorldCenter => this.IsRoot ? base.WorldCenter : this.Root.Position + this.LocalCenter.Rotate(this.Root.Rotation);
        public override Vector2 LocalCenter => _localCenter;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            _rootFixtures = new Queue<FixtureContainer>();

            this.OnChainChanged += this.HandleChainChanged;
        }

        protected override void Release()
        {
            base.Release();

            while (_rootFixtures.Any())
                _rootFixtures.Dequeue().Destroy();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnChainChanged -= this.HandleChainChanged;
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
                fixtures.Dequeue().Destroy();


            // Create new fixtures for all vertices contained in the configuration
            foreach(ShipPartShapeContext shape in this.Context.Shapes)
            {
                Vertices vertices = new Vertices(shape.Vertices);
                vertices.Transform(this.LocalTransformation);
                fixtures.Enqueue(root.BuildFixture(new PolygonShape(vertices, shape.Density), this));
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
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if(value != default)
            {
                this.AddFixtures(this.Root, _rootFixtures);

                _localCenter = this.IsRoot ? base.LocalCenter : Vector2.Transform(this.Context.Shapes.Centeroid, this.LocalTransformation);
            }    
        }
        #endregion
    }
}
