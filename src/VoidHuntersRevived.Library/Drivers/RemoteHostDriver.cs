using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers
{
    /// <summary>
    /// Simple helper driver that implement InitializeRemote
    /// & ReleaseRemote methods that will only be invoked 
    /// when the driven item connects to a remote peer
    /// </summary>
    /// <typeparam name="TDriven"></typeparam>
    public class RemoteHostDriver<TDriven> : Driver<TDriven>
        where TDriven : Driven
    {
        #region Private Fields
        private Settings _settings;

        private HostType _initialHostType;
        private NetworkAuthorization _initialNetworkAuthorization;
        #endregion

        #region Protected Fields 
        protected Settings settings => _settings;

        protected HostType initialHostType => _initialHostType;
        protected NetworkAuthorization initialNetworkAuthorization => _initialNetworkAuthorization;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(TDriven driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _settings);

            _initialHostType = settings.Get<HostType>();
            _initialNetworkAuthorization = settings.Get<NetworkAuthorization>();

            if (_initialHostType == HostType.Remote)
                this.InitializeRemote(driven, provider, _initialNetworkAuthorization);
        }

        protected virtual void InitializeRemote(TDriven driven, ServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            // 
        }

        protected override void Release(TDriven driven)
        {
            base.Release(driven);

            if (_initialHostType == HostType.Remote)
                this.ReleaseRemote(driven, _initialNetworkAuthorization);

            _settings = null;
        }

        protected virtual void ReleaseRemote(TDriven driven, NetworkAuthorization networkAuthorization)
        {
            // 
        }
        #endregion
    }
}
