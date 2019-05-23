using FarseerPhysics.Dynamics;
using Guppy;
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
        private Body _serverBody;
        private VoidHuntersClientWorldScene _scene;
        private FarseerEntity _entity;

        #region Constructors
        public ClientFarseerEntityDriver(VoidHuntersClientWorldScene scene, FarseerEntity entity, ILogger logger) : base(entity, logger)
        {
            _scene = scene;
            _entity = entity;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Create a new body within the server world to represent the server render of the current entity
            // _serverBody = _entity.CreateBody(_scene.ServerWorld, _entity.Body.Position, _entity.Body.Rotation, _entity.Body.BodyType);
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
        #endregion
    }
}
