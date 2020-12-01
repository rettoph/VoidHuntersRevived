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

        private Double _fireTime;
        private Single _curRecoil;
        #endregion

        #region Public Properties
        /// <summary>
        /// All internal joints related to the current weapon.
        /// </summary>
        public IReadOnlyDictionary<Body, RevoluteJoint> Joints => _joints;

        /// <inheritdoc />
        public override Matrix WorldTransformation => Matrix.CreateTranslation(_curRecoil, 0, 0) * Matrix.CreateRotationZ(this.Get<Single>(b => this.GetJoint(b)?.JointAngle ?? 0)) * base.WorldTransformation;

        /// <summary>
        /// The amount (in farseer units) the gun should recoil when fired.
        /// </summary>
        public Single Recoil { get; set; } = 0.3f;
        #endregion

        #region Events
        public event ValidateEventDelegate<Ship, ShipPart> ValidateFire;
        public event OnEventDelegate<Weapon, Ammunition> OnFire;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);
            _ammunitions = new List<Ammunition>();
            _joints = new Dictionary<Body, RevoluteJoint>();
            _fireTimer = new ActionTimer(400);

            _provider = provider;

            // Create new shapes for the part
            this.Configuration.Vertices.ForEach(v => this.BuildFixture(new PolygonShape(v, this.Configuration.Density), this));

            this.OnChainChanged += this.HandleChainChanged;

            // Create new default joints as needed
            this.UpdateJoints();
        }

        protected override void Release()
        {
            base.Release();

            this.OnChainChanged -= this.HandleChainChanged;
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

            // Update all ammunitions
            _ammunitions.TryUpdateAll(gameTime);
        }

        /// <summary>
        /// Update the fire timer, and trigger a fire
        /// event if needed.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateFire(GameTime gameTime)
            => _fireTimer.Update(gameTime, this.TryFire);

        /// <summary>
        ///  Specifically used to update the wepaons position. Only 
        ///  applicable when the root is not connected to a ship.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdatePosition(GameTime gameTime)
        {
            _curRecoil = 0;
            this.MaleConnectionNode.Target?.TryPreview(this);
        }

        private void UpdateAim(GameTime gameTime)
        {
            // Update the recoil as needed
            if (this.Recoil > 0 && _curRecoil < 0.99f)
                _curRecoil = MathHelper.Lerp(this.Recoil, 0, MathHelper.Min(1f, (Single)((gameTime.TotalGameTime.TotalMilliseconds - _fireTime) / (_fireTimer.Interval / 1.5))));

            this.TryAim(this.Chain.Ship.Target);
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
            if (!this.IsRoot)
            {
                this.CollidesWith = this.Root.CollidesWith;
                this.CollisionCategories = this.Root.CollisionCategories;
                this.IgnoreCCDWith = this.Root.IgnoreCCDWith;
            }
        }

        /// <summary>
        /// Clean which update methods should be invoked each frame...
        /// </summary>
        private void CleanUpdate()
        {
            this.OnUpdate -= this.UpdateAim;
            this.OnUpdate -= this.UpdatePosition;

            if (this.Chain.Ship == default(Ship)) // When there is no ship, update position directly...
                this.OnUpdate += this.UpdatePosition;
            else // When there is a ship, attempt to aim the gun
                this.OnUpdate += this.UpdateAim;
        }

        public void TryFire(GameTime gameTime)
        {
            if (this.ValidateFire.Validate(this.Chain.Ship, this, true))
            {
                _fireTime = gameTime.TotalGameTime.TotalMilliseconds;
                _curRecoil = this.Recoil;
                var ammo = this.Fire(_provider);
                _ammunitions.Add(ammo);
                
                this.OnFire?.Invoke(this, ammo);
            }
        }

        protected abstract Ammunition Fire(ServiceProvider provider);
        #endregion

        #region Event Handlers
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            _dirtyJoints = true;

            if (old != default(Chain))
            {
                old.OnUpdate -= this.TryUpdate;

                old.Root.OnCollidesWithChanged -= this.HandleRootCollisionChanged;
                old.Root.OnCollisionCategoriesChanged -= this.HandleRootCollisionChanged;
                old.Root.OnIgnoreCCDWithChanged -= this.HandleRootCollisionChanged;
                old.OnShipChanged -= this.HandleRootShipChanged;
            }

            if (value != default(Chain))
            {
                value.OnUpdate += this.TryUpdate;

                value.Root.OnCollidesWithChanged += this.HandleRootCollisionChanged;
                value.Root.OnCollisionCategoriesChanged += this.HandleRootCollisionChanged;
                value.Root.OnIgnoreCCDWithChanged += this.HandleRootCollisionChanged;
                value.OnShipChanged += this.HandleRootShipChanged;
            }

            // Clean default wepaon data
            this.CleanCollision();
            this.CleanUpdate();
        }

        private void HandleRootCollisionChanged(BodyEntity sender, Category arg)
            => this.CleanCollision();

        private void HandleRootShipChanged(Chain sender, Ship old, Ship value)
            => this.CleanUpdate();
        #endregion
    }
}
