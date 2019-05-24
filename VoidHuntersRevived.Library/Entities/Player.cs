using Guppy;
using Guppy.Configurations;
using Guppy.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// the payer class represents an in game instance that contains 
    /// </summary>
    public class Player : NetworkEntity
    {
        #region Public Attributes
        public FarseerEntity Bridge { get; private set; }
        #endregion

        #region Events
        public event EventHandler<FarseerEntity> OnBridgeUpdated;
        #endregion

        #region Constructors
        public Player(EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
        }
        public Player(Guid id, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, scene, provider, logger)
        {
        }
        #endregion

        #region Utility Methods
        public void UpdateBridge(FarseerEntity bridge)
        {
            this.logger.LogDebug($"Updateing Player({this.Id}) bridge to {bridge.GetType().Name}({bridge.Id})");

            this.Bridge = bridge;

            this.OnBridgeUpdated?.Invoke(this, this.Bridge);
        }
        #endregion
    }
}
