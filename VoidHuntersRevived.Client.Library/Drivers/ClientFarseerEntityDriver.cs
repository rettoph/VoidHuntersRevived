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
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _entity.ActionHandlers.Add("update:position", this.HandleUpdatePositionAction);

            // Bind event handlers
            _entity.OnCollidesWithChanged += this.HandleCollidesWithChanged;
            _entity.OnCollisionCategoriesChanged += this.CollidesCollisionCategoriesChanged;
            _entity.OnIsSensorChanged += this.HandleIsSensorChanged;
            _entity.OnSleepingAllowedChanged += this.HandleSleepingAllowedChanged;
            _entity.OnPhysicsEnabledChanged += this.HandlePhysicsEnabledChanged;
            _entity.OnBodyCreated += this.HandleBodyCreated;
            _entity.OnFixtureCreated += this.HandleFixtureCreated;
            _entity.OnFixtureDestroyed += this.HandleFixtureDestroyed;
            _entity.OnLinearImpulseApplied += this.HandleLinearImpulseApplied;
            _entity.OnAngularImpulseApplied += this.HandleAngularImpulseApplied;
            _entity.OnSetTransform += this.HandleSetTransform;
            _entity.OnRead += this.HandleRead;
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
                _entity.Position = Vector2.Lerp(
                    _entity.Position, 
                    _server.Bodies[_entity].Position, 
                    _lerpStrength);

                _entity.Rotation = MathHelper.Lerp(
                    _entity.Rotation, 
                    _server.Bodies[_entity].Rotation, 
                    _lerpStrength);

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
            _server.Bodies[_entity].Position = obj.ReadVector2();
            _server.Bodies[_entity].Rotation = obj.ReadSingle();
            _server.Bodies[_entity].LinearVelocity = obj.ReadVector2();
            _server.Bodies[_entity].AngularVelocity = obj.ReadSingle();

            _server.Bodies[_entity].Awake = true;
        }
        #endregion

        #region Event Handlers
        private void HandleBodyCreated(object sender, Body e)
        {
            _server.Bodies.Add(_entity, e.DeepClone(_server.World));
        }

        /// <summary>
        /// When the client render recieves a fixture,
        /// duplocate it on the server render too
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shape"></param>
        private void HandleFixtureCreated(object sender, Fixture fixture)
        {
            var sFixture = fixture.CloneOnto(_server.Bodies[_entity]);

            _clientServerFixtureTable.Add(fixture, sFixture);
        }

        /// <summary>
        /// When the client render destroyed a fixture,
        /// duplicate it on the server render too
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shape"></param>
        private void HandleFixtureDestroyed(object sender, Fixture fixture)
        {
            _server.Bodies[_entity].DestroyFixture(_clientServerFixtureTable[fixture]);
            _clientServerFixtureTable.Remove(fixture);
        }

        private void HandleAngularImpulseApplied(object sender, float impulse)
        {
            _server.Bodies[_entity].ApplyAngularImpulse(impulse);
        }

        private void HandleLinearImpulseApplied(object sender, Vector2 impulse)
        {
            _server.Bodies[_entity].ApplyLinearImpulse(impulse);
        }

        private void CollidesCollisionCategoriesChanged(object sender, Category category)
        {
            _server.Bodies[_entity].CollisionCategories = category;
        }

        private void HandleCollidesWithChanged(object sender, Category category)
        {
            _server.Bodies[_entity].CollidesWith = category;
        }

        private void HandleIsSensorChanged(object sender, bool e)
        {
            _server.Bodies[_entity].IsSensor = e;
        }

        private void HandleSleepingAllowedChanged(object sender, bool e)
        {
            _server.Bodies[_entity].SleepingAllowed = e;
        }

        private void HandlePhysicsEnabledChanged(object sender, bool e)
        {
            _server.Bodies[_entity].Enabled = e;
        }

        private void HandleSetTransform(object sender, Body e)
        {
            _server.Bodies[_entity].SetTransform(e.Position, e.Rotation);
        }

        private void HandleRead(object sender, NetworkEntity e)
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

            _server.Bodies[_entity].Dispose();
            _server.Bodies.Remove(_entity);

            _entity.OnCollidesWithChanged -= this.HandleCollidesWithChanged;
            _entity.OnCollisionCategoriesChanged -= this.CollidesCollisionCategoriesChanged;
            _entity.OnIsSensorChanged -= this.HandleIsSensorChanged;
            _entity.OnSleepingAllowedChanged -= this.HandleSleepingAllowedChanged;
            _entity.OnPhysicsEnabledChanged -= this.HandlePhysicsEnabledChanged;
            _entity.OnBodyCreated -= this.HandleBodyCreated;
            _entity.OnFixtureCreated -= this.HandleFixtureCreated;
            _entity.OnFixtureDestroyed -= this.HandleFixtureDestroyed;
            _entity.OnLinearImpulseApplied -= this.HandleLinearImpulseApplied;
            _entity.OnAngularImpulseApplied -= this.HandleAngularImpulseApplied;
            _entity.OnSetTransform -= this.HandleSetTransform;
            _entity.OnRead -= this.HandleRead;
        }
    }
}
