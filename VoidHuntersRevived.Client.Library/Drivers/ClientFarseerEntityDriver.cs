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
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientFarseerEntityDriver : Driver
    {
        public static Dictionary<FarseerEntity, Body> ServerBody;

        private Dictionary<Fixture, Fixture> _clientServerFixtureTable;
        private Body _serverBody {
            get { return ClientFarseerEntityDriver.ServerBody[_entity]; }
            set { ClientFarseerEntityDriver.ServerBody[_entity] = value; }
        }
        private VoidHuntersClientWorldScene _scene;
        private FarseerEntity _entity;
        private Single _lerpStrength;

        #region Constructors
        public ClientFarseerEntityDriver(VoidHuntersClientWorldScene scene, FarseerEntity entity, IServiceProvider provider, ILogger logger) : base(entity, provider, logger)
        {
            _scene = scene;
            _entity = entity;
        }
        static ClientFarseerEntityDriver()
        {
            ClientFarseerEntityDriver.ServerBody = new Dictionary<FarseerEntity, Body>();
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
            _entity.OnFixtureCreated += this.HandleFixtureCreated;
            _entity.OnFixtureDestroyed += this.HandleFixtureDestroyed;
            _entity.OnLinearImpulseApplied += this.HandleLinearImpulseApplied;
            _entity.OnAngularImpulseApplied += this.HandleAngularImpulseApplied;
            _entity.OnRead += this.HandleRead;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _lerpStrength = 0.05f;

            // Create a new body within the server world to represent the server render of the current entity
            _serverBody = _entity.CreateBody(_scene.ServerWorld, _entity.Position, _entity.Rotation);
            _serverBody.SleepingAllowed = _entity.SleepingAllowed;
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if(!_entity.Focused.Value && _serverBody.Awake)
            {
                _entity.Position = Vector2.Lerp(_entity.Position, _serverBody.Position, _lerpStrength);
                _entity.Rotation = MathHelper.Lerp(_entity.Rotation, _serverBody.Rotation, _lerpStrength);
                _entity.LinearVelocity = Vector2.Lerp(_entity.LinearVelocity, _serverBody.LinearVelocity, _lerpStrength);
                _entity.AngularVelocity = MathHelper.Lerp(_entity.AngularVelocity, _serverBody.AngularVelocity, _lerpStrength);
            }
        }
        #endregion

        #region Action Handlers
        private void HandleUpdatePositionAction(NetIncomingMessage obj)
        {
            _serverBody.Position = obj.ReadVector2();
            _serverBody.Rotation = obj.ReadSingle();
            _serverBody.LinearVelocity = obj.ReadVector2();
            _serverBody.AngularVelocity = obj.ReadSingle();

            _serverBody.Awake = true;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the client render recieves a fixture,
        /// duplocate it on the server render too
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shape"></param>
        private void HandleFixtureCreated(object sender, Fixture fixture)
        {
            var sFixture = fixture.CloneOnto(_serverBody);

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
            _serverBody.DestroyFixture(_clientServerFixtureTable[fixture]);
            _clientServerFixtureTable.Remove(fixture);
        }

        private void HandleAngularImpulseApplied(object sender, float impulse)
        {
            _serverBody.ApplyAngularImpulse(impulse);
        }

        private void HandleLinearImpulseApplied(object sender, Vector2 impulse)
        {
            _serverBody.ApplyLinearImpulse(impulse);
        }

        private void CollidesCollisionCategoriesChanged(object sender, Category category)
        {
            _serverBody.CollisionCategories = category;
        }

        private void HandleCollidesWithChanged(object sender, Category category)
        {
            _serverBody.CollidesWith = category;
        }

        private void HandleIsSensorChanged(object sender, bool e)
        {
            _serverBody.IsSensor = e;
        }

        private void HandleSleepingAllowedChanged(object sender, bool e)
        {
            _serverBody.SleepingAllowed = e;
        }

        private void HandlePhysicsEnabledChanged(object sender, bool e)
        {
            _serverBody.Enabled = e;
        }

        private void HandleRead(object sender, NetworkEntity e)
        {
            _serverBody.Position = _entity.Position;
            _serverBody.Rotation = _entity.Rotation;
            _serverBody.LinearVelocity = _entity.LinearVelocity;
            _serverBody.AngularVelocity = _entity.AngularVelocity;
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            _serverBody.Dispose();
            ClientFarseerEntityDriver.ServerBody.Remove(_entity);

            _entity.OnCollidesWithChanged -= this.HandleCollidesWithChanged;
            _entity.OnCollisionCategoriesChanged -= this.CollidesCollisionCategoriesChanged;
            _entity.OnFixtureCreated -= this.HandleFixtureCreated;
            _entity.OnFixtureDestroyed -= this.HandleFixtureDestroyed;
            _entity.OnLinearImpulseApplied -= this.HandleLinearImpulseApplied;
            _entity.OnAngularImpulseApplied -= this.HandleAngularImpulseApplied;
            _entity.OnRead -= this.HandleRead;
        }
    }
}
