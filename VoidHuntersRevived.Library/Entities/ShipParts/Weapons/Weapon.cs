using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using GalacticFighters.Library.Configurations;
using GalacticFighters.Library.Extensions;
using GalacticFighters.Library.Utilities;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts.Weapons
{
    public class Weapon : RigidShipPart
    {
        #region Private Fields
        private Body _barrel;
        private World _world;
        private Body _owner;
        private WeldJoint _joint;
        #endregion

        #region Protected Attributes
        protected new WeaponConfiguration config { get; private set; }
        #endregion

        #region Public Attributes
        /// <summary>
        /// The weapons current live barrel, if any
        /// </summary>
        public Body Barrel { get => _barrel; }
        #endregion

        #region Constructors
        public Weapon(World world)
        {
            _world = world;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.SetUpdateOrder(300);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Save the configuration
            this.config = this.Configuration.Data as WeaponConfiguration;

            // Create a new Barrel for the weapon
            _barrel = BodyFactory.CreatePolygon(_world, this.config.Barrel, 0.1f);
            _barrel.BodyType = BodyType.Dynamic;

            this.Events.TryAdd<Body>("position:changed", this.HandlePositionChanged);

            this.IsLive = true;
        }

        protected override void Initialize()
        {
            base.Initialize();


        }

        public override void Dispose()
        {
            base.Dispose();

            _barrel.Dispose();
        }
        #endregion

        protected override void UpdateChainPlacement()
        {
            base.UpdateChainPlacement();

            if (_joint != null) // Delete the old joint, if there was one
                _world.RemoveJoint(_joint);
            if (_owner != null) // Restore barrel colission with its parent
                _barrel.RestoreCollisionWith(_owner);

            // Update the barrel's position
            this.UpdateBarrelPosition();

            // Create a new weld join between the barrelt and its root body
            _joint = JointFactory.CreateWeldJoint(
                _world, 
                this.Root.GetBody(), 
                _barrel, 
                Vector2.Transform(this.config.BodyAnchor, this.LocalTransformation), 
                this.config.BarrelAnchor, 
                false);
            // Save the barrels root, so we can restore colission in the future
            _owner = this.Root.GetBody();
            _barrel.IgnoreCollisionWith(_owner);

            // Update the barrel's collision info
            _barrel.CollidesWith = this.Root.CollidesWith;
            _barrel.CollisionCategories = this.Root.CollisionCategories;
        }

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // When reserved, instantly update barrel position so the joint doesnt have to
            if (this.Root.Reserverd.Value)
                this.UpdateBarrelPosition();
        }
        #endregion

        private void UpdateBarrelPosition()
        {
            // Calculate the barrelts proper position based on the defined anchor points.
            var position = this.Root.Position + Vector2.Transform(this.config.BodyAnchor + this.config.BarrelAnchor, this.LocalTransformation * Matrix.CreateRotationZ(this.Root.Rotation));
            // Update the barrels position
            _barrel.SetTransform(position, this.Rotation + MathHelper.Pi);
        }

        #region Event Handlers
        private void HandlePositionChanged(object sender, Body arg)
        {
            this.UpdateBarrelPosition();
        }
        #endregion
    }
}
