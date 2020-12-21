﻿using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using VoidHuntersRevived.Library.Extensions.Aether;
using Microsoft.Xna.Framework;
using Guppy.IO.Extensions.log4net;
using VoidHuntersRevived.Library.Utilities.Farseer;
using Guppy.Extensions.Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Collision.Shapes;

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

            this.OnChainChanged += this.HandleChainChanged;
        }

        protected override void Release()
        {
            base.Release();

            while (_fixtures.Any())
                _fixtures.Dequeue().Destroy();

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
            {
                this.log.Verbose(() => $"Destroying RigidShipPart({this.Id}) Fixture");
                fixtures.Dequeue().Destroy();
            }
                

            // Create new fixtures for all vertices contained in the configuration
            this.Configuration.Vertices.ForEach(data =>
            {
                this.log.Verbose(() => $"Creating Fixture for RigidShipPart({this.Id}) on ShipPart({this.Root.Id})");

                Vertices vertices = new Vertices(data);
                vertices.Transform(this.LocalTransformation);
                fixtures.Enqueue(root.BuildFixture(new PolygonShape(vertices, this.Configuration.Density), this));
            });
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
            this.AddFixtures(this.Root, _fixtures);

            _localCenter = this.IsRoot ? base.LocalCenter : Vector2.Transform(this.Configuration.Centeroid, this.LocalTransformation);
        }
        #endregion
    }
}
