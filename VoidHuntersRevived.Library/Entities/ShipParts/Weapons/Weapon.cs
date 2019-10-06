using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using GalacticFighters.Library.Configurations;
using GalacticFighters.Library.Extensions;
using GalacticFighters.Library.Utilities;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        /// <summary>
        /// The root object saved internally
        /// the last time the current
        /// weapons chain placement was updated.
        /// </summary>
        private ShipPart _root;
        private RevoluteJoint _joint;
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
            this.Events.TryAdd<NetIncomingMessage>("read", this.HandleRead);

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

            if (_root != default(ShipPart))
            {
                _root.Events.TryRemove<Body>("position:changed", this.HandlePositionChanged);
                _root.Events.TryRemove<NetIncomingMessage>("read", this.HandleRead);
                _root.Events.TryRemove<Category>("collision-categories:changed", this.HandleCollisionCategoriesChanged);
            }

            _root = this.Root;
            _root.Events.TryAdd<Body>("position:changed", this.HandlePositionChanged);
            _root.Events.TryAdd<NetIncomingMessage>("read", this.HandleRead);

            this.UpdateJoint(ref _joint, this.Root.GetBody(), _barrel, _world);
        }

        private void HandleCollisionCategoriesChanged(object sender, Category arg)
        {
            throw new NotImplementedException();
        }

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // When reserved, instantly update barrel position so the joint doesnt have to
            if (this.Root.Reserverd.Value)
                this.UpdateBarrelPosition();

            this.UpdateBarrelAngle();
        }
        #endregion

        #region Helper Methods
        public void UpdateBarrelAngle()
        {
            this.UpdateBarrelAngle(_joint, this.Root.GetBody());
        }
        public void UpdateBarrelAngle(RevoluteJoint joint, Body root)
        {
            if (this.Root.IsBridge)
            { // Only update the angle if the ship is root
                var target = this.Root.BridgeFor.Target;
                var position = root.Position + Vector2.Transform(this.config.BodyAnchor, this.LocalTransformation * Matrix.CreateRotationZ(root.Rotation));
                var offset = target - position;
            
                // Calculate the target angle we wish to aim towards
                var angle = MathHelper.WrapAngle((Single)Math.Atan2(offset.Y, offset.X) - root.Rotation - this.LocalRotation - this.MaleConnectionNode.LocalRotation - MathHelper.Pi);
                // Sanatize angle, ensuring it is between upper and lower  joint limits
                angle = Math.Max(joint.LowerLimit, Math.Min(joint.UpperLimit, angle));

                // The difference between the barrels current orientation and the mouse
                var diff = angle - joint.JointAngle;

                joint.MotorSpeed = diff * (Single)(1000 / 32);
            }
        }

        /// <summary>
        /// Automatically update the current weapons barrel position
        /// </summary>
        public void UpdateBarrelPosition()
        {
            this.UpdateBarrelPosition(this.Root.GetBody(), _barrel);
        }
        /// <summary>
        /// Update a farseer bodies position assuming that the given body
        /// is the current weapons barrel.
        /// </summary>
        /// <param name="root">Containing root body</param>
        /// <param name="barrel">The assumed barrel</param>
        public void UpdateBarrelPosition(Body root, Body barrel)
        {
            // Calculate the barrels proper position based on the defined anchor points.
            var position = root.Position + Vector2.Transform(this.config.BodyAnchor + this.config.BarrelAnchor, this.LocalTransformation * Matrix.CreateRotationZ(root.Rotation));
            // Update the barrels position
            barrel.SetTransform(position, this.Root.IsBridge ? barrel.Rotation : root.Rotation + this.LocalRotation + MathHelper.Pi);
        } 

        public void UpdateJoint(ref RevoluteJoint joint, Body root, Body barrel, World world)
        {
            if (joint != null) // Destroy the old joint, if it exists
                world.RemoveJoint(joint);

            // By default, reset the barrel rotation relative to the given root body
            barrel.Rotation = root.Rotation + this.LocalRotation + MathHelper.Pi;

            // Update the recieved barrel's position
            this.UpdateBarrelPosition(root, barrel);

            joint = JointFactory.CreateRevoluteJoint(
                world,
                root,
                barrel,
                Vector2.Transform(this.config.BodyAnchor, this.LocalTransformation),
                this.config.BarrelAnchor,
                false);

            joint.CollideConnected = false;
            joint.MotorEnabled = true;
            joint.MaxMotorTorque = 0.5f;
            joint.MotorSpeed = 0f;
            joint.LowerLimit = -this.config.Range / 2;
            joint.UpperLimit = this.config.Range / 2;
            joint.LimitEnabled = true;

            // Update the barrel's collision info
            barrel.CollidesWith = this.Root.CollidesWith;
            barrel.CollisionCategories = this.Root.CollisionCategories;
        }
        #endregion

        #region Event Handlers
        private void HandlePositionChanged(object sender, Body arg)
        {
            this.UpdateBarrelPosition();
            this.UpdateBarrelAngle();
        }

        private void HandleRead(object sender, NetIncomingMessage im)
        {
            this.UpdateBarrelPosition();
            this.UpdateBarrelAngle();
        }
        #endregion
    }
}
