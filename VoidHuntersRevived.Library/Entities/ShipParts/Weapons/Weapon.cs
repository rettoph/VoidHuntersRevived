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

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// The base weapon class.
    /// </summary>
    public class Weapon : ShipPart
    {
        #region Private Fields
        private RevoluteJoint _joint;
        #endregion

        #region Public Properties
        public RevoluteJoint Joint { get => _joint; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.DefaultColor = Color.Red;

            // Automatically add the weapons fixtures...
            this.AddFixtures(this.Body);

            this.Events.TryAdd<ChainUpdate>("chain:updated", this.HandleChainUpdated);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Update(this.Root.Body, this.Body, this.Joint);
        }

        public void Update(Body root, Body weapon, RevoluteJoint joint)
        {
            if (this.Root.Ship != default(Ship))
                this.UpdateTarget(this.Root.Ship.WorldTarget, joint, weapon);
            else
                this.UpdatePosition(root, weapon);
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
                body.CreateFixture(new PolygonShape(vertices, 0.01f), this);
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
                joint.LowerLimit = -MathHelper.PiOver2 / 2;
                joint.UpperLimit = MathHelper.PiOver2 / 2;
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
            weapon.SetTransformIgnoreContacts(
                position: root.Position + Vector2.Transform(this.MaleConnectionNode.LocalPosition, this.LocalTransformation * Matrix.CreateRotationZ(root.Rotation)),
                angle: root.Rotation + this.LocalRotation);
        }

        /// <summary>
        /// Update the weapons current target.
        /// </summary>
        /// <param name="target">The world position the weapon should target</param>
        /// <param name="joint"></param>
        /// <param name="root"></param>
        /// <returns>Whether or not the weapon is able to reach the requested target.</returns>
        public Boolean UpdateTarget(Vector2 target, RevoluteJoint joint, Body root)
        {
            if(joint != default(RevoluteJoint) && !(this.Controller is Chunk))
            { // Only update the target if the weapon is not in a controller...
                // Calculate the offset beteen the joints position and the requested target.
                var offset = target - joint.WorldAnchorB;
                // Calculate the joint should approach relative to the weapon body
                var angle = MathHelper.Clamp(
                    value: MathHelper.WrapAngle(
                        angle: (Single)Math.Atan2(offset.Y, offset.X) - root.Rotation),
                    min: this.Joint.LowerLimit,
                    max: this.Joint.UpperLimit);

                // Calculate the different between the required angle and the current angle
                var diff = angle - joint.JointAngle;

                // Set the joints speed
                joint.MotorSpeed = diff * (1000f / 32f);

                return angle == this.Joint.LowerLimit || angle == this.Joint.UpperLimit;
            }

            return false;
        }
        #endregion

        #region Event Handlers
        private void HandleChainUpdated(object sender, ChainUpdate arg)
        {
            this.UpdateJoint(this.Root.Body, this.Body, this.world, ref _joint);
        }
        #endregion
    }
}
