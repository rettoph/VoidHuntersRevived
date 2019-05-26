using Guppy;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Implementations;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerFarseerEntityDriver : Driver
    {
        private FarseerEntity _entity;
        private Double _lastUpdatePositionAction;
        private Double _updatePositionActionRate;

        public ServerFarseerEntityDriver(FarseerEntity entity, IServiceProvider provider, ILogger logger) : base(entity, provider, logger)
        {
            _entity = entity;
        }

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _updatePositionActionRate = 150;
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            _lastUpdatePositionAction += gameTime.ElapsedGameTime.TotalMilliseconds;

            if(_entity.Body.Awake && _lastUpdatePositionAction >= _updatePositionActionRate)
            {
                this.SendUpdatePositionAction();
                _lastUpdatePositionAction = _lastUpdatePositionAction % _updatePositionActionRate;
            }
            else
            {
                _lastUpdatePositionAction = _updatePositionActionRate;
            }
        }

        #region Utility Methods
        private void SendUpdatePositionAction()
        {
            var action = _entity.CreateActionMessage("update:position");
            action.Write(_entity.Body.Position);
            action.Write(_entity.Body.Rotation);
            action.Write(_entity.Body.LinearVelocity);
            action.Write(_entity.Body.AngularVelocity);
        }
        #endregion

        #region Event Handlers

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
