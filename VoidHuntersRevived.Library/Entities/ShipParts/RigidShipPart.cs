using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Represents a ShipPart that transplants it's fixtures
    /// directly onto the root most ShipPart.
    /// </summary>
    public abstract class RigidShipPart : ShipPart
    {
        #region Private Fields
        private Queue<Fixture> _fixtures;
        #endregion

        #region Public Fields
        public override Vector2 Position { get => this.IsRoot ? base.Position : this.Root.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(this.Root.Rotation)); }
        public override Single Rotation { get => this.IsRoot ? base.Rotation : this.Root.Rotation + this.LocalRotation; }

        public override Vector2 WorldCenter { get => this.Root.Position + Vector2.Transform(this.LocalCenter, Matrix.CreateRotationZ(this.Root.Rotation)); }
        public override Vector2 LocalCenter { get => this.IsRoot ? this.Body.LocalCenter : Vector2.Transform(this.Configuration.GetData<ShipPartConfiguration>().Centeroid, this.LocalTransformation); }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _fixtures = new Queue<Fixture>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Events.TryAdd<ChainUpdate>("chain:updated", this.HandleChainUpdated);
        }

        public override void Dispose()
        {
            base.Dispose();

            _fixtures.Clear();
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
        public void AddFixturesToRoot(Body root, Queue<Fixture> fixtures)
        {
            // Auto dispose of any fixtures within the given queue
            while (fixtures.Any())
                fixtures.Dequeue().Dispose();

            // Create new fixtures for all vertices contained in the configuration
            this.Configuration.GetData<ShipPartConfiguration>().Vertices.ForEach(data =>
            {
                Vertices vertices = new Vertices(data);
                vertices.Transform(this.LocalTransformation);
                fixtures.Enqueue(root.CreateFixture(new PolygonShape(vertices, 0.5f), this));
            });

            // Auto call the current controller's Setup method
            this.Controller?.SetupBody(this.Root, root);
        }
        #endregion

        #region Event Handlers
        private void HandleChainUpdated(object sender, ChainUpdate arg)
        {
            // If the chain update event is moving downwards, we should recreate the internal fixtures
            if (arg.HasFlag(ChainUpdate.Down))
                this.AddFixturesToRoot(this.Root.Body, _fixtures);
        }
        #endregion
    }
}
