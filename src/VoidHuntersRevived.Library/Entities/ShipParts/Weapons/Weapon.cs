using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Collections;
using Guppy.Interfaces;
using Guppy.Lists;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Extensions.Aether;
using tainicom.Aether.Physics2D.Common;

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
        /// Simple timer to manage the weapons fire rate.
        /// </summary>
        private ActionTimer _fireTimer;

        private ServiceProvider _provider;
        private EntityList _entities;

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

        /// <summary>
        /// Indicates that the weapon is currently aiming at the requested target
        /// if false, we shouldnt fire.
        /// </summary>
        public Boolean TargetInRange { get; private set; }
        #endregion

        #region Events
        public event ValidateEventDelegate<Weapon, Ship> ValidateFire;
        public event OnEventDelegate<Weapon, Ammunition> OnFire;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.ValidateFire += Weapon.HandleValidateFire;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);
            _joints = new Dictionary<Body, RevoluteJoint>();
            _fireTimer = new ActionTimer(400);

            _provider = provider;
            provider.Service(out _entities);

            // Create new shapes for the part
            foreach (Vertices vertices in this.Configuration.Vertices)
                this.BuildFixture(new PolygonShape(vertices, this.Configuration.Density), this);

            this.OnChainChanged += this.HandleChainChanged;

            // Create new default joints as needed
            this.CleanJoints();
        }

        protected override void Release()
        {
            base.Release();

            this.CleanUpdate();
            this.OnChainChanged -= this.HandleChainChanged;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.ValidateFire -= Weapon.HandleValidateFire;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

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
        #endregion

        #region Helper Methods
        /// <summary>
        /// Refresh/create the internal joints as needed.
        /// </summary>
        private void CleanJoints()
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
                    joint.MaxMotorTorque = 0.75f;
                    joint.MotorSpeed = 0.0f;
                    joint.LowerLimit = -(2 / 2);
                    joint.UpperLimit = (2 / 2);
                    joint.LimitEnabled = true;
                    joint.Enabled = this.Chain.Ship != default;

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
            // TODO: Helper extension method?
            joint.BodyA.World.Remove(joint);
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
                    var angle = MathHelper.WrapAngle((Single)Math.Atan2(offset.Y, offset.X) - this.MaleConnectionNode.WorldRotation);

                    // Calculate the current different in angle
                    var diff = angle - joint.JointAngle;

                    if(diff + 1 < joint.LowerLimit)
                    {
                        body.Rotation = this.Root.GetChild(this.GetParent(body)).Rotation + this.LocalRotation - joint.UpperLimit;
                    }
                    else if(joint.UpperLimit < diff - 1)
                    {
                        body.Rotation = this.Root.GetChild(this.GetParent(body)).Rotation + this.LocalRotation + joint.UpperLimit;
                    }
                    else
                    {
                        // Set the joints speed...
                        joint.MotorSpeed = diff * (1000f / 64f);
                        this.TargetInRange = MathHelper.Clamp(angle, joint.LowerLimit, joint.UpperLimit) == angle;
                    }
                }
            });
        }

        public RevoluteJoint GetJoint(Body body)
        {
            if (_joints.ContainsKey(body))
                return _joints[body];

            return default;
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
            }
        }

        /// <summary>
        /// Clean which update methods should be invoked each frame...
        /// </summary>
        private void CleanUpdate()
        {
            this.OnUpdate -= this.UpdateAim;
            this.OnUpdate -= this.UpdatePosition;

            if(this.Chain?.Ship != default)// When there is a ship, attempt to aim the gun
                this.OnUpdate += this.UpdateAim;
            else if(!this.IsRoot) // When there is no ship but its in a chain, update position directly...
                this.OnUpdate += this.UpdatePosition;
        }

        public void TryFire(GameTime gameTime)
        {
            _fireTimer.Update(gameTime, v => v && this.ValidateFire.Validate(this, this.Chain.Ship, true), gt =>
            {
                _fireTime = gameTime.TotalGameTime.TotalMilliseconds;
                _curRecoil = this.Recoil;
                var ammo = this.Fire(_provider, _entities);
                ammo.ShooterId = this.Chain.Id;

                this.OnFire?.Invoke(this, ammo);
            });
        }

        protected abstract Ammunition Fire(ServiceProvider provider, EntityList entities);
        #endregion

        #region Event Handlers
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if (old != default)
            {
                old.Root.OnCollidesWithChanged -= this.HandleRootCollisionChanged;
                old.Root.OnCollisionCategoriesChanged -= this.HandleRootCollisionChanged;
                old.OnShipChanged -= this.HandleRootShipChanged;
                old.OnUpdate -= this.TryUpdate;
            }

            if (value != default)
            {
                value.Root.OnCollidesWithChanged += this.HandleRootCollisionChanged;
                value.Root.OnCollisionCategoriesChanged += this.HandleRootCollisionChanged;
                value.OnShipChanged += this.HandleRootShipChanged;
                value.OnUpdate += this.TryUpdate;
            }

            // Clean default wepaon data
            this.CleanCollision();
            this.CleanUpdate();
            this.CleanJoints();
        }

        private void HandleRootCollisionChanged(BodyEntity sender, Category arg)
            => this.CleanCollision();

        private void HandleRootShipChanged(Chain sender, Ship old, Ship value)
        {
            this.CleanUpdate();

            // Enable or disable joints based on the Ship status..
            foreach (RevoluteJoint joint in _joints.Values)
                joint.Enabled = value != default;
        }

        private static bool HandleValidateFire(Weapon sender, Ship args)
            => sender.TargetInRange;
        #endregion
    }
}
