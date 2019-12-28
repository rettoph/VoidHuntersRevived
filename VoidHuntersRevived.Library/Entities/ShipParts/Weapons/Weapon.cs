using FarseerPhysics.Dynamics.Joints;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Library.Configurations;
using Guppy.Extensions.Collection;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Entities.Controllers;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Extensions.System;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// The base weapon class.
    /// </summary>
    public abstract class Weapon : ShipPart
    {
        #region Private Fields
        private RevoluteJoint _joint;
        private ActionTimer _fireTimer;
        #endregion

        #region Public Properties
        public RevoluteJoint Joint { get => _joint; }
        /// <summary>
        /// indicates that the weapon was succesfully able
        /// to reach the desired target last update.
        /// </summary>
        public Boolean OnTarget { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _fireTimer = new ActionTimer(this.Configuration.GetData<WeaponConfiguration>().FireRate);

            this.DefaultColor = Color.Red;

            // Automatically add the weapons fixtures...
            this.AddFixtures(this.Body);

            this.OnChainUpdated += this.HandleChainUpdated;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.OnChainUpdated -= this.HandleChainUpdated;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.OnTarget = this.Update(this.Root.Body, this.Body, this.Joint);
            this.TryFire(gameTime);
        }

        public Boolean Update(Body root, Body weapon, RevoluteJoint joint)
        {
            if (this.Root.Ship != default(Ship))
                return this.UpdateTarget(this.Root.Ship.WorldTarget, joint, root, weapon);
            else
                this.UpdatePosition(root, weapon);

            return false;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Add the defined weapon's fixtures into the recieved body.
        /// </summary>
        /// <param name="body"></param>
        public void AddFixtures(Body body)
        {
            // Create new fixtures for all vertices contained in the configuration
            this.Configuration.GetData<ShipPartConfiguration>().Vertices.ForEach(vertices =>
            {
                body.CreateFixture(new PolygonShape(vertices, 0.5f), this);
            });
        }

        /// <summary>
        /// Update the connecting joint between the weapon's root
        /// & the weapon
        /// </summary>
        /// <param name="root"></param>
        /// <param name="weapon"></param>
        /// <param name="joint"></param>
        public void UpdateJoint(Body root, Body weapon, World world, ref RevoluteJoint joint)
        {
            // Auto remove the joint if i already exists
            if (joint != default(RevoluteJoint))
                world.RemoveJoint(joint);

            if(!this.IsRoot)
            { // Only create a joint if the weapon is not already the root...
                // By default update the weapons current position...
                this.UpdatePosition(root, weapon);

                // Create a new joint
                joint = JointFactory.CreateRevoluteJoint(
                    world: world,
                    bodyA: root,
                    bodyB: weapon,
                    anchorA: Vector2.Transform(this.MaleConnectionNode.LocalPosition, this.LocalTransformation),
                    anchorB: this.MaleConnectionNode.LocalPosition,
                    false);

                // Setup the joint
                joint.MotorEnabled = true;
                joint.CollideConnected = false;
                joint.MaxMotorTorque = 0.5f;
                joint.MotorSpeed = 0.0f;
                joint.LimitEnabled = true;
                joint.LowerLimit = -this.Configuration.GetData<WeaponConfiguration>().SwivelRange / 2;
                joint.UpperLimit = this.Configuration.GetData<WeaponConfiguration>().SwivelRange / 2;
            }
        }

        /// <summary>
        /// Instantly update the weapon's current position relative to the
        /// recieved root body.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="weapon"></param>
        public void UpdatePosition(Body root, Body weapon)
        {
            this.UpdatePosition(root, weapon, root.Rotation + this.LocalRotation);
        }
        /// <summary>
        /// Update the weapon's current position relative to the
        /// recieved root body with a custom angle.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="weapon"></param>
        /// <param name="rotation"></param>
        public void UpdatePosition(Body root, Body weapon, Single rotation)
        {
            var offset = Vector2.Transform(this.MaleConnectionNode.LocalPosition, Matrix.CreateRotationZ(rotation) * this.MaleConnectionNode.LocalRotationMatrix);
            var translation = Matrix.CreateTranslation(offset.X, offset.Y, 0);

            weapon.SetTransformIgnoreContacts(
                position: root.Position + Vector2.Transform(
                    position: this.MaleConnectionNode.LocalPosition, 
                    matrix:
                    this.LocalTransformation *
                    Matrix.CreateRotationZ(root.Rotation) *
                    translation),
                angle: rotation);
        }

        /// <summary>
        /// Update the weapons current target.
        /// </summary>
        /// <param name="target">The world position the weapon should target</param>
        /// <param name="joint"></param>
        /// <param name="root"></param>
        /// <returns>Whether or not the weapon is able to reach the requested target.</returns>
        public Boolean UpdateTarget(Vector2 target, RevoluteJoint joint, Body root, Body weapon)
        {
            if(joint != default(RevoluteJoint) && !(this.Controller is Chunk))
            { // Only update the target if the weapon is not in a controller...
                this.UpdatePosition(root, weapon, weapon.Rotation);
                if (this.Health > 0)
                {
                    // Calculate the offset beteen the joints position and the requested target.
                    var offset = target - joint.WorldAnchorB;
                    // Calculate the joint should approach relative to the weapon body
                    var angle = MathHelper.Clamp(
                        value: MathHelper.WrapAngle(
                            angle: (Single)Math.Atan2(offset.Y, offset.X) - this.MaleConnectionNode.LocalRotation - this.MaleConnectionNode.Target.WorldRotation),
                        min: this.Joint.LowerLimit,
                        max: this.Joint.UpperLimit);

                    // Calculate the different between the required angle and the current angle
                    var diff = angle - joint.JointAngle;

                    // Set the joints speed
                    joint.MotorSpeed = diff * (1000f / 32f);

                    return !(angle == this.Joint.LowerLimit || angle == this.Joint.UpperLimit);
                }
            }

            return false;
        }
        #endregion

        #region Fire Methods
        public void TryFire(GameTime gameTime)
        {
            _fireTimer.Update(
                gameTime: gameTime,
                action: () => this.Fire(),
                filter: (triggered) => triggered && this.OnTarget && this.Health > 0 && this.Root.Ship != default(Ship) && this.Root.Ship.Firing); 
        }

        protected abstract void Fire();
        #endregion

        #region Event Handlers
        private void HandleChainUpdated(object sender, ChainUpdate arg)
        {
            this.UpdateJoint(this.Root.Body, this.Body, this.world, ref _joint);
        }
        #endregion
    }
}
