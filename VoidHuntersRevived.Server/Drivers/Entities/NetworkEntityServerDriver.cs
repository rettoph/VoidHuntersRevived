using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Server.Utilities;

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    /// <summary>
    /// Driver that will track an internal entities vitals status.
    /// If an entity can send vitals data it will mark the vitals
    /// as dirty and utilize the VitalsManager to queue the entity
    /// for cleaning. The vitals manager itself will then wait
    /// N amount of time before flushing all buffered entities
    /// </summary>
    [IsDriver(typeof(NetworkEntity))]
    internal sealed class NetworkEntityServerDriver : Driver<NetworkEntity>
    {
        #region Static Properties
        private static Double VitalsPingRate { get; set; } = 150;
        #endregion

        #region Private Fields
        private ActionTimer _vitalPingTimer;
        private Boolean _dirty;
        private VitalsManager _vitals;
        #endregion

        #region Constructor
        public NetworkEntityServerDriver(VitalsManager vitals, NetworkEntity driven) : base(driven)
        {
            _vitals = vitals;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _vitalPingTimer = new ActionTimer(NetworkEntityServerDriver.VitalsPingRate);
        }
        #endregion

        #region Helper Methods
        private void WriteVitals(NetOutgoingMessage om)
        {
            om.Write(this.driven);
            this.driven.TryWiteVitals(om);

            _dirty = false;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _vitalPingTimer.Update(
                gameTime: gameTime,
                filter: triggered => !_dirty && this.driven.CanSendVitals(triggered),
                action: () =>
                {
                    _dirty = true;
                    _vitals.Enqueue(this.WriteVitals);
                });
        }
        #endregion
    }
}
