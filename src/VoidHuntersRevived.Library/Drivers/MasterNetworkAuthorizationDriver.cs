using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers
{
    public abstract class MasterNetworkAuthorizationDriver<TDriven> : RemoteHostDriver<TDriven>
        where TDriven : Driven
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(TDriven driven, ServiceProvider provider, NetworkAuthorization networkAuthorization)
            => this.InitializeRemote(driven, provider);

        protected virtual void InitializeRemote(TDriven driven, ServiceProvider provider)
        {
            // 
        }

        protected override void ReleaseRemote(TDriven driven, NetworkAuthorization networkAuthorization)
               => this.ReleaseRemote(driven);

        protected virtual void ReleaseRemote(TDriven driven)
        {
            //
        }
        #endregion
    }
}
