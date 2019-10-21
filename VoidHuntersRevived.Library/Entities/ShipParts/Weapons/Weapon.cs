using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Extensions;
using VoidHuntersRevived.Library.Utilities;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Utilities.Controllers;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    public abstract class Weapon : RigidShipPart
    {
        #region Private Fields
        /// <summary>
        /// The world the weapon resides in.
        /// </summary>
        private World _world;

        /// <summary>
        /// The weapons barrel.
        /// </summary>
        private Body _barrel;
        
        /// <summary>
        /// The root object saved internally
        /// the last time the current
        /// weapons chain placement was updated.
        /// </summary>
        private ShipPart _root;

        /// <summary>
        /// The live joint connecting the base of the gun to the barrel
        /// </summary>
        private RevoluteJoint _joint;

        private Interval _interval;

        private Double _lastFire;
        #endregion

        #region Protected Attributes
        protected new WeaponConfiguration config { get; private set; }
        #endregion

        #region Public Attributes
        /// <summary>
        /// The weapons current live barrel, if any
        /// </summary>
        public Body Barrel { get => _barrel; }

        public Single JointAngle { get => _joint.JointAngle; }

        public Vector2 WorldBodyAnchor { get => _joint.WorldAnchorA; }

        public Boolean OnTarget { get; private set; }
        #endregion

        #region Constructors
        public Weapon(Interval interval, World world)
        {
            _world = world;
            _interval = interval;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.Events.Register<Vector2>("target:updated");
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Save the configuration
            this.config = this.Configuration.Data as WeaponConfiguration;

            // Create a new Barrel for the weapon
            _barrel = BodyFactory.CreatePolygon(_world, this.config.Barrel, 0.1f, Vector2.Zero, 0, BodyType.Dynamic, this);

            this.Events.TryAdd<Body>("position:changed", this.HandlePositionChanged);
            this.Events.TryAdd<NetIncomingMessage>("read", this.HandleRead);
            this.Events.TryAdd<Controller>("controller:changed", this.HandleControllerChanged);
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

        #region Utility Methods
        protected override void UpdateChainPlacement()
        {
            base.UpdateChainPlacement();

            // Update joint data
            this.UpdateJoint(ref _joint, this.Root.GetBody(), _barrel, _world);

            // Update barrel information
            _barrel.CollidesWith = this.Root.CollidesWith;
            _barrel.CollisionCategories = this.Root.CollisionCategories;
            _barrel.IgnoreCCDWith = this.Root.IgnoreCCDWith;

            if (_root != default(ShipPart))
            { // Remove old events
                _root.Events.TryRemove<Boolean>("body-enabled:changed", this.HandleBodyEnabledChanged);
                _root.Events.TryRemove<Body>("position:changed", this.HandlePositionChanged);
                _root.Events.TryRemove<NetIncomingMessage>("read", this.HandleRead);
            }

            // Save new events
            _root = this.Root;
            _root.Events.TryAdd<Boolean>("body-enabled:changed", this.HandleBodyEnabledChanged);
            _root.Events.TryAdd<Body>("position:changed", this.HandlePositionChanged);
            _root.Events.TryAdd<NetIncomingMessage>("read", this.HandleRead);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.UpdateBarrelPosition();

            _lastFire += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        #endregion

        #region Helper Methods
        public void UpdateBarrelTarget(Vector2 target)
        {
            this.UpdateBarrelTarget(target, _joint, this.Root.GetBody());

            this.Events.TryInvoke<Vector2>(this, "target:updated", target);
        }
        public void UpdateBarrelTarget(Vector2 target, RevoluteJoint joint, Body root)
        {
            if (!(this.Controller is Chunk))
            {
                var position = root.Position + Vector2.Transform(this.config.BodyAnchor, this.LocalTransformation * Matrix.CreateRotationZ(root.Rotation));
                var offset = target - position;

                // Calculate the target angle we wish to aim towards
                var angle = MathHelper.WrapAngle((Single)Math.Atan2(offset.Y, offset.X) - root.Rotation - this.LocalRotation - this.MaleConnectionNode.LocalRotation - MathHelper.Pi);

                if (joint.LowerLimit < angle && angle < joint.UpperLimit)
                {
                    this.OnTarget = true;
                }
                else
                {
                    this.OnTarget = false;
                    // Sanatize angle, ensuring it is between upper and lower  joint limits
                    angle = Math.Max(joint.LowerLimit, Math.Min(joint.UpperLimit, angle));
                }

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
            barrel.SetTransformIgnoreContacts(ref position, this.Root.Controller is Chunk ? root.Rotation + this.LocalRotation + MathHelper.Pi : barrel.Rotation);
        } 

        public void UpdateJoint(ref RevoluteJoint joint, Body root, Body barrel, World world)
        {
            if (joint != null) // Destroy the old joint, if it exists
                world.RemoveJoint(joint);

            // By default, reset the barrel rotation relative to the given root body
            this.UpdateBarrelPosition(root, barrel);
            barrel.SetTransformIgnoreContacts(barrel.Position, root.Rotation + this.LocalRotation + MathHelper.Pi);

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
            barrel.Enabled = this.Root.BodyEnabled;
        }

        public virtual Boolean TryFire()
        {
            if(_lastFire > 300)
            {
                _lastFire = 0;

                return true;
            }

            return false;
        }
        #endregion

        #region Event Handlers
        private void HandlePositionChanged(object sender, Body arg)
        {
            this.UpdateBarrelPosition();
        }

        private void HandleRead(object sender, NetIncomingMessage im)
        {
            this.UpdateBarrelPosition();
            this.UpdateBarrelTarget(this.WorldBodyAnchor);
        }

        private void HandleBodyEnabledChanged(object sender, bool arg)
        {
            _barrel.Enabled = this.Root.Enabled;
        }

        private void HandleControllerChanged(object sender, Controller arg)
        {
            _barrel.CollidesWith = this.CollidesWith;
            _barrel.CollisionCategories = this.CollisionCategories;
            _barrel.IgnoreCCDWith = this.IgnoreCCDWith;
        }
        #endregion
    }
}
