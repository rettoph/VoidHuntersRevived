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
    /// when the 
    /// </summary>
    /// <typeparam name="TDriven"></typeparam>
    public class RemoteHostDriver<TDriven> : Driver<TDriven>
        where TDriven : Driven
    {
        #region Private Fields
        private Settings _settings;
        #endregion

        #region Protected Fields 
        protected Settings settings => _settings;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(TDriven driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _settings);

            if(settings.Get<HostType>() == HostType.Remote)
                this.InitializeRemote(driven, provider);
        }

        protected virtual void InitializeRemote(TDriven driven, ServiceProvider provider)
        {
            // 
        }

        protected override void Release(TDriven driven)
        {
            base.Release(driven);

            if (settings.Get<HostType>() == HostType.Remote)
                this.ReleaseRemote(driven);
        }

        protected virtual void ReleaseRemote(TDriven driven)
        {
            // 
        }
        #endregion
    }
}
