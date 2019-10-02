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

            this.IsLive = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _barrel = BodyFactory.CreatePolygon(_world, this.config.Barrel, 0.1f);
            _barrel.BodyType = BodyType.Dynamic;

            this.Events.TryAdd<Body>("position:changed", this.HandlePositionChanged);
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
            if (this.Root.IsBridge)
            {
                _barrel.CollidesWith = CollisionCategories.ActiveCollidesWith;
                _barrel.CollisionCategories = CollisionCategories.ActiveCollisionCategories;
            }
            else
            {
                _barrel.CollidesWith = CollisionCategories.PassiveCollidesWith;
                _barrel.CollisionCategories = CollisionCategories.PassiveCollisionCategories;
            }
        }

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // When reserved, instantly update barrel position so the joint doesnt have to
            if (this.Reserverd.Value)
                this.UpdateBarrelPosition();
        }
        #endregion

        private void UpdateBarrelPosition()
        {
            Console.WriteLine("Updating!");
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
