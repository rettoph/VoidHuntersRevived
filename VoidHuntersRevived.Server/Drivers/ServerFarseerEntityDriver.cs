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

        public ServerFarseerEntityDriver(FarseerEntity entity, IServiceProvider provider) : base(entity, provider)
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

        protected override void update(GameTime gameTime)
        {
            _lastUpdatePositionAction += gameTime.ElapsedGameTime.TotalMilliseconds;

            if(!_entity.Focused && _entity.Awake && _lastUpdatePositionAction >= _updatePositionActionRate)
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
            _entity.WritePositionData(action);
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
