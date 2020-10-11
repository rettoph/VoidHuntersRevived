using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Collections;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// The base weapon class, implementing all
    /// default weapong functionality ranging from
    /// aiming to firing.
    /// 
    /// All "shots" from weapons should be maintined
    /// internally.
    /// </summary>
    public abstract class Weapon : ShipPart
    {
        #region Private Fields
        /// <summary>
        /// The primary farseer join managing
        /// </summary>
        private Dictionary<Body, RevoluteJoint> _joints;

        /// <summary>
        /// When true, joints will be updated next frame
        /// </summary>
        private Boolean _dirtyJoints;

        /// <summary>
        /// Simple timer to manage the weapons fire rate.
        /// </summary>
        private ActionTimer _fireTimer;

        /// <summary>
        /// An internal list of all live ammuntions within the weapon.
        /// </summary>
        private List<Ammunition> _ammunitions;

        private ServiceProvider _provider;
        #endregion

        #region Public Properties
        /// <summary>
        /// All internal joints related to the current weapon.
        /// </summary>
        public IReadOnlyDictionary<Body, RevoluteJoint> Joints => _joints;

        /// <inheritdoc />
        public override Matrix WorldTransformation => Matrix.CreateRotationZ(this.Get<Single>(b => this.GetJoint(b)?.JointAngle ?? 0)) * base.WorldTransformation;
        #endregion

        #region Events
        public ValidateEventDelegate<Ship, ShipPart> ValidateFire;
        public GuppyEventHandler<Weapon, Ammunition> OnFire;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);
            _ammunitions = new List<Ammunition>();
            _joints = new Dictionary<Body, RevoluteJoint>();
            _fireTimer = new ActionTimer(250);

            _provider = provider;

            // Create new shapes for the part
            this.Configuration.Vertices.ForEach(v => this.BuildFixture(new PolygonShape(v, 0.01f), this));

            this.OnRootChanged += this.HandleRootChanged;

            // Create new default joints as needed
            this.UpdateJoints();
        }

        protected override void Release()
        {
            base.Release();

            this.OnRootChanged -= this.HandleRootChanged;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_dirtyJoints)
            {
                this.UpdateJoints();
                _dirtyJoints = false;
            }

            // Attempt to update the weapons target...
            if (this.Root.Ship != default(Ship))
            {
                this.TryAim(this.Root.Ship.Target);

                // For now just continiously try to fire
                // _fireTimer.Update(gameTime, this.TryFire);

                // Update all ammunitions
                _ammunitions.TryUpdateAll(gameTime);
            }
            else
                this.MaleConnectionNode.Target?.TryPreview(this);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _ammunitions.TryDrawAll(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Refresh/create the internal joints as needed.
        /// </summary>
        private void UpdateJoints()
        {
            // Auto dispose of each pre-existing joint
            while (_joints.Any())
                this.RemoveJoint(_joints.First().Key, _joints.First().Value);

            // Reposition the current weapon at a 0deg angle
            this.MaleConnectionNode.Target?.TryPreview(this);

            if (!this.IsRoot)
            { // Only bother creating any joints if the current Weapon is not a root piece
                this.Do(body =>
                { // Create a joint for each internal body
                    // Load the world & root body positions...
                    var world = this.GetParent(body);
                    var root = this.Root.GetChild(world);

                    // Create a new joint...
                    var joint = JointFactory.CreateRevoluteJoint(
                        world: world,
                        bodyA: root,
                        bodyB: body,
                        anchorA: Vector2.Transform(this.MaleConnectionNode.LocalPosition, this.LocalTransformation),
                        anchorB: this.MaleConnectionNode.LocalPosition);

                    // Finialize joint setup...
                    joint.MotorEnabled = true;
                    joint.CollideConnected = false;
                    joint.MaxMotorTorque = 0.5f;
                    joint.MotorSpeed = 0.0f;
                    joint.LimitEnabled = true;
                    joint.Enabled = true;
                    joint.LowerLimit = -(2 / 2);
                    joint.UpperLimit = (2 / 2);

                    // Save the joint internally
                    _joints[body] = joint;
                });
            }
        }

        /// <summary>
        /// Destroy the recieved joint instance.
        /// </summary>
        /// <param name="p"></param>
        private void RemoveJoint(Body body, RevoluteJoint joint)
        {
            this.GetParent(body).RemoveJoint(joint);
            _joints.Remove(body);
        }

        /// <summary>
        /// Attempt to aim the weapon at a given target.
        /// </summary>
        /// <param name="target"></param>
        public void TryAim(Vector2 target)
        {
            this.Do(body =>
            { // Iterate through each internal body...
                var joint = this.GetJoint(body);

                if (joint != default(RevoluteJoint))
                {
                    // Calculate the current offset between the join & the target
                    var offset = joint.WorldAnchorB - target;

                    // Calculate the angle the joint should be to reach the current target...
                    var angle = MathHelper.Clamp(
                        value: MathHelper.WrapAngle(
                            angle: (Single)Math.Atan2(offset.Y, offset.X) - this.MaleConnectionNode.WorldRotation),
                        min: joint.LowerLimit,
                        max: joint.UpperLimit);

                    // Calculate the current different in angle
                    var diff = angle - joint.JointAngle;

                    // Set the joints speed...
                    joint.MotorSpeed = diff * (1000f / 64f);

                    // Console.WriteLine($"World Anchor: ({joint.WorldAnchorB.X.ToString("00.0")}, {joint.WorldAnchorB.Y.ToString("00.0")}); Target: ({target.X.ToString("00.0")}, {target.Y.ToString("00.0")}); Offset: ({offset.X.ToString("00.0")}, {offset.Y.ToString("00.0")}); Target Angle: {(angle).ToString("000.00")}; Joint Angle: {(joint.JointAngle).ToString("000.00")}; Diff: {(diff).ToString("000.00")}");
                }
            });
        }

        public RevoluteJoint GetJoint(Body body)
        {
            if (_joints.ContainsKey(body))
                return _joints[body];

            return default(RevoluteJoint);
        }

        /// <summary>
        /// Refresh the current weapons collisions
        /// </summary>
        private void CleanCollision()
        {
            // Automatically set the weapons collision values to match the root.
            this.CollidesWith = this.Root.CollidesWith;
            this.CollisionCategories = this.Root.CollisionCategories;
            this.IgnoreCCDWith = this.Root.IgnoreCCDWith;
        }

        public void TryFire()
        {
            if (this.ValidateFire?.Validate(this.Root.Ship, this) ?? true)
            {
                var ammo = this.Fire(_provider);
                _ammunitions.Add(ammo);

                this.OnFire?.Invoke(this, ammo);
            }
        }

        protected abstract Ammunition Fire(ServiceProvider provider);
        #endregion

        #region Event Handlers
        private void HandleRootChanged(ShipPart sender, ShipPart old, ShipPart value)
        {
            _dirtyJoints = true;

            if (old != default(ShipPart) && old != this)
            {
                old.OnUpdate -= this.TryUpdate;

                value.OnCollidesWithChanged -= this.HandleCollisionChanged;
                value.OnCollisionCategoriesChanged -= this.HandleCollisionChanged;
                value.OnIgnoreCCDWithChanged -= this.HandleCollisionChanged;
            }

            if (value != default(ShipPart) && value != this)
            {
                value.OnUpdate += this.TryUpdate;

                value.OnCollidesWithChanged += this.HandleCollisionChanged;
                value.OnCollisionCategoriesChanged += this.HandleCollisionChanged;
                value.OnIgnoreCCDWithChanged += this.HandleCollisionChanged;

                this.CleanCollision();
            }
        }

        private void HandleCollisionChanged(BodyEntity sender, Category arg)
            => this.CleanCollision();
        #endregion
    }
}
