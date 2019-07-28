using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Implementations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientFarseerEntityDriver : Driver
    {
        private Dictionary<Fixture, Fixture> _clientServerFixtureTable;
        private VoidHuntersClientWorldScene _scene;
        private FarseerEntity _entity;
        private Single _lerpStrength;
        private ServerRender _server;

        #region Constructors
        public ClientFarseerEntityDriver(ServerRender server, VoidHuntersClientWorldScene scene, FarseerEntity entity, IServiceProvider provider) : base(entity, provider)
        {
            _server = server;
            _scene = scene;
            _entity = entity;
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            _clientServerFixtureTable = new Dictionary<Fixture, Fixture>();

            // Bind event handlers
            _entity.Events.AddHandler("changed:collides-with", this.HandleCollidesWithChanged);
            _entity.Events.AddHandler("changed:collision-categories", this.HandleCollisionCategoriesChanged);
            _entity.Events.AddHandler("changed:collision-group", this.HandleCollisionGroupChanged);
            _entity.Events.AddHandler("changed:is-sensor", this.HandleIsSensorChanged);
            _entity.Events.AddHandler("changed:sleeping-allowed", this.HandleSleepingAllowedChanged);
            _entity.Events.AddHandler("changed:physics-enabled", this.HandlePhysicsEnabledChanged);
            _entity.Events.AddHandler("changed:linear-damping", this.HandleAngularDampingChanged);
            _entity.Events.AddHandler("changed:angular-damping", this.HandleLinearDampingChanged);
            _entity.Events.AddHandler("applied:linear-impulse", this.HandleLinearImpulseApplied);
            _entity.Events.AddHandler("applied:angular-impulse", this.HandleAngularImpulseApplied);
            _entity.Events.AddHandler("applied:force", this.HandleForceApplied);
            _entity.Events.AddHandler("set:transform", this.HandleSetTransform);
            _entity.Events.AddHandler("created:body", this.HandleBodyCreated);
            _entity.Events.AddHandler("created:fixture", this.HandleFixtureCreated);
            _entity.Events.AddHandler("destroyed:fixture", this.HandleFixtureDestroyed);
            _entity.Events.AddHandler("on:read", this.HandleRead);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _entity.Actions.AddHandler("update:position", this.HandleUpdatePositionAction);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _lerpStrength = 0.05f;
        }
        #endregion

        #region Frame Methods
        protected override void draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        protected override void update(GameTime gameTime)
        {
            if(!_entity.Focused.Value && (_server.Bodies[_entity].Awake || _entity.Awake))
            {
                // Update the entity position & rotation
                _entity.Position = Vector2.Lerp(
                    _entity.Position,
                    _server.Bodies[_entity].Position,
                    _lerpStrength);
                _entity.Rotation = MathHelper.Lerp(
                    _entity.Rotation,
                    _server.Bodies[_entity].Rotation,
                    _lerpStrength);

                // Update the entity velocities
                _entity.LinearVelocity = Vector2.Lerp(
                    _entity.LinearVelocity, 
                    _server.Bodies[_entity].LinearVelocity, 
                    _lerpStrength);
                _entity.AngularVelocity = MathHelper.Lerp(
                    _entity.AngularVelocity, 
                    _server.Bodies[_entity].AngularVelocity, 
                    _lerpStrength);
            }
        }
        #endregion

        #region Action Handlers
        private void HandleUpdatePositionAction(NetIncomingMessage obj)
        {
            _server.Bodies[_entity].SetTransform(obj.ReadVector2(), obj.ReadSingle());
            _server.Bodies[_entity].LinearVelocity = obj.ReadVector2();
            _server.Bodies[_entity].AngularVelocity = obj.ReadSingle();

            _server.Bodies[_entity].Awake = true;
        }
        #endregion

        #region Event Handlers
        private void HandleBodyCreated(Object arg)
        {
            _server.Bodies.Add(_entity, (arg as Body).DeepClone(_server.World));
        }

        /// <summary>
        /// When the client render recieves a fixture,
        /// duplicate it on the server render too
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shape"></param>
        private void HandleFixtureCreated(Object arg)
        {
            var fixture = arg as Fixture;
            var sFixture = fixture.CloneOnto(_server.Bodies[_entity]);

            _clientServerFixtureTable.Add(fixture, sFixture);
        }

        /// <summary>
        /// When the client render destroyed a fixture,
        /// duplicate it on the server render too
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shape"></param>
        private void HandleFixtureDestroyed(Object arg)
        {
            var fixture = arg as Fixture;
            _server.Bodies[_entity].DestroyFixture(_clientServerFixtureTable[fixture]);
            _clientServerFixtureTable.Remove(fixture);
        }

        private void HandleAngularImpulseApplied(Object arg)
        {
            _server.Bodies[_entity].ApplyAngularImpulse((Single)arg);
        }

        private void HandleLinearImpulseApplied(Object arg)
        {
            _server.Bodies[_entity].ApplyLinearImpulse((Vector2)arg);
        }

        private void HandleCollisionCategoriesChanged(Object arg)
        {
            _server.Bodies[_entity].CollisionCategories = (Category)arg;
        }

        private void HandleCollidesWithChanged(Object arg)
        {
            _server.Bodies[_entity].CollidesWith = (Category)arg;
        }

        private void HandleCollisionGroupChanged(Object arg)
        {
            _server.Bodies[_entity].CollisionGroup = (Int16)arg;
        }

        private void HandleIsSensorChanged(Object arg)
        {
            _server.Bodies[_entity].IsSensor = (Boolean)arg;
        }

        private void HandleSleepingAllowedChanged(Object arg)
        {
            _server.Bodies[_entity].SleepingAllowed = (Boolean)arg;
        }

        private void HandlePhysicsEnabledChanged(Object arg)
        {
            _server.Bodies[_entity].Enabled = (Boolean)arg;
        }

        private void HandleSetTransform(Object arg)
        {
            _server.Bodies[_entity].SetTransform(_entity.Position, _entity.Rotation);
        }

        private void HandleForceApplied(Object arg)
        {
            var e = arg as ForceEventArgs;
            _server.Bodies[_entity].ApplyForce(e.Force, e.Point);
        }

        private void HandleAngularDampingChanged(Object arg)
        {
            _server.Bodies[_entity].AngularDamping = (Single)arg;
        }

        private void HandleLinearDampingChanged(Object arg)
        {
            _server.Bodies[_entity].LinearDamping = (Single)arg;
        }

        private void HandleRead(Object e)
        {
            _server.Bodies[_entity].Position = _entity.Position;
            _server.Bodies[_entity].Rotation = _entity.Rotation;
            _server.Bodies[_entity].LinearVelocity = _entity.LinearVelocity;
            _server.Bodies[_entity].AngularVelocity = _entity.AngularVelocity;
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            if (_server.Bodies.ContainsKey(_entity))
            {
                _server.Bodies[_entity].Dispose();
                _server.Bodies.Remove(_entity);
            }

            _entity.Events.RemoveHandler("changed:collides-with", this.HandleCollidesWithChanged);
            _entity.Events.RemoveHandler("changed:collision-categories", this.HandleCollisionCategoriesChanged);
            _entity.Events.RemoveHandler("changed:collision-group", this.HandleCollisionGroupChanged);
            _entity.Events.RemoveHandler("changed:is-sensor", this.HandleIsSensorChanged);
            _entity.Events.RemoveHandler("changed:sleeping-allowed", this.HandleSleepingAllowedChanged);
            _entity.Events.RemoveHandler("changed:physics-enabled", this.HandlePhysicsEnabledChanged);
            _entity.Events.RemoveHandler("changed:linear-damping", this.HandleAngularDampingChanged);
            _entity.Events.RemoveHandler("changed:angular-damping", this.HandleLinearDampingChanged);
            _entity.Events.RemoveHandler("applied:linear-impulse", this.HandleLinearImpulseApplied);
            _entity.Events.RemoveHandler("applied:angular-impulse", this.HandleAngularImpulseApplied);
            _entity.Events.RemoveHandler("applied:force", this.HandleForceApplied);
            _entity.Events.RemoveHandler("set:transform", this.HandleSetTransform);
            _entity.Events.RemoveHandler("created:body", this.HandleBodyCreated);
            _entity.Events.RemoveHandler("created:fixture", this.HandleFixtureCreated);
            _entity.Events.RemoveHandler("destroyed:fixture", this.HandleFixtureDestroyed);
            _entity.Events.RemoveHandler("on:read", this.HandleRead);
        }
    }
}
