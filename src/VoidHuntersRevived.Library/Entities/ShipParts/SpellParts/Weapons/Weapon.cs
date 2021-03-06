﻿using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.System.Collections;
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
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Extensions.Aether;
using tainicom.Aether.Physics2D.Common;
using Guppy.Extensions.System;
using Guppy.Extensions.Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Utilities.Farseer;

namespace VoidHuntersRevived.Library.Entities.ShipParts.SpellParts.Weapons
{
    /// <summary>
    /// The base weapon class, implementing all
    /// default weapong functionality ranging from
    /// aiming to firing.
    /// 
    /// All "shots" from weapons should be maintined
    /// internally.
    /// </summary>
    public abstract class Weapon : SpellPart
    {
        #region Private Fields
        /// <summary>
        /// The primary farseer join managing
        /// </summary>
        private Dictionary<Body, RevoluteJoint> _joints;

        private ServiceProvider _provider;
        private EntityList _entities;

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
        /// Indicates that the weapon is currently aiming at the requested target
        /// if false, we shouldnt fire.
        /// </summary>
        public Boolean TargetInRange { get; private set; }

        public new WeaponContext Context { get; private set; }
        public override Single Rotation => this.IsRoot ? base.Rotation : base.Rotation + this.Get(b => _joints[b].JointAngle);
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnValidateCast += Weapon.HandleValidateCast;
            this.OnChainChanged += Weapon.HandleChainChanged;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _joints = new Dictionary<Body, RevoluteJoint>();

            _provider = provider;
            provider.Service(out _entities);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            // Create new default joints as needed
            this.CleanJoints();
        }

        protected override void Release()
        {
            base.Release();

            _provider = null;
            _entities = null;

            // Auto dispose of each pre-existing joint
            while (_joints.Any())
                this.RemoveJoint(_joints.First().Key, _joints.First().Value);
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnValidateCast -= Weapon.HandleValidateCast;
            this.OnChainChanged -= Weapon.HandleChainChanged;
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
            if (this.Context.Recoil > 0 && _curRecoil < 0.99f)
                _curRecoil = MathHelper.Lerp(this.Context.Recoil, 0, MathHelper.Min(1f, (Single)((gameTime.TotalGameTime.TotalSeconds - this.LastCastTimestamp) / (this.Context.SpellCooldown / 1.5))));

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

            if (!this.IsRoot)
            { // Only bother creating any joints if the current Weapon is not a root piece
                this.MaleConnectionNode.Target?.TryPreview(this);
                this.Do(body =>
                { // Create a joint for each internal body
                    if (body.World == default || !body.FixtureList.Any())
                        return; // Do nothing if World is null.

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
                    joint.CollideConnected = false;
                    joint.MaxMotorTorque = 2f;
                    joint.MotorSpeed = 0.0f;
                    joint.MotorEnabled = true;
                    joint.LowerLimit = -(this.Context.SwivelRange / 2);
                    joint.UpperLimit = (this.Context.SwivelRange / 2);
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
            joint.TryRemove();
            _joints.Remove(body);
        }

        /// <summary>
        /// Attempt to aim the weapon at a given target.
        /// </summary>
        /// <param name="target"></param>
        public void TryAim(Vector2 target)
        {
            if (this.Health == 0)
                return;

            this.Do(body =>
            { // Iterate through each internal body...
                var joint = this.GetJoint(body);

                if (joint != default(RevoluteJoint))
                {
                    // Calculate the current offset between the join & the target
                    var offset = joint.WorldAnchorB - target;

                    // Calculate the angle the joint should be to reach the current target...
                    var angle = MathHelper.WrapAngle((Single)Math.Atan2(offset.Y, offset.X) - this.MaleConnectionNode.GetWordRotation(joint.BodyA));

                    this.TargetInRange = MathHelper.Clamp(
                        value: angle, 
                        min: joint.LowerLimit - this.Context.MaximumOffsetFireRange, 
                        max: joint.UpperLimit + this.Context.MaximumOffsetFireRange) == angle;

                    if (!this.TargetInRange) // Dont bother aiming at all if its out of range
                        return; // This prevents weird buggy looking gun swivels when the mouse is behind the gun

                    // Calculate the current different in angle
                    var diff = angle - joint.JointAngle;
                    joint.MotorSpeed = diff * (1000f / 64f);
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

            if(this.Chain?.Ship != default) // When there is a ship, attempt to aim the gun
                this.OnUpdate += this.UpdateAim;
            else if(!this.IsRoot) // When there is no ship but its in a chain, update position directly...
                this.OnUpdate += this.UpdatePosition;
        }

        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as WeaponContext;
        }

        protected override void BuildInternalFixtures(Queue<FixtureContainer> fixtures)
        {
            // Create new shapes for the part
            foreach (ShapeContext shape in this.Context.InnerShapes)
                if (shape.Solid)
                    fixtures.Enqueue(this.BuildFixture(new PolygonShape(shape.Vertices, this.Context.Density), this));
        }
        #endregion

        #region Event Handlers
        private static void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if(sender is Weapon weapon)
            {
                if (old != default)
                {
                    old.Root.OnCollidesWithChanged -= weapon.HandleRootCollisionChanged;
                    old.Root.OnCollisionCategoriesChanged -= weapon.HandleRootCollisionChanged;
                    old.OnShipChanged -= weapon.HandleRootShipChanged;
                    old.OnUpdate -= weapon.TryUpdate;
                }

                if (value != default)
                {
                    value.Root.OnCollidesWithChanged += weapon.HandleRootCollisionChanged;
                    value.Root.OnCollisionCategoriesChanged += weapon.HandleRootCollisionChanged;
                    value.OnShipChanged += weapon.HandleRootShipChanged;
                    value.OnUpdate += weapon.TryUpdate;

                    // Clean default wepaon data
                    weapon.CleanCollision();
                    weapon.CleanUpdate();
                    weapon.CleanJoints();
                }
            }
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

        private static bool HandleValidateCast(SpellPart sender, GameTime args)
            => sender is Weapon weapon && weapon.TargetInRange;
        #endregion
    }
}
