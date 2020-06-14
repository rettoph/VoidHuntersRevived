using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers
{
    public class BaseAuthorizationDriver<TDriven> : Driver<TDriven>
        where TDriven : Driven
    {
        #region Private Fields
        private GameAuthorization _authorization;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            _authorization = provider.GetService<Settings>().Get<GameAuthorization>();

            this.Configure(provider, _authorization);
            switch(_authorization)
            {
                case GameAuthorization.Full:
                    this.ConfigureFull(provider);
                    break;
                case GameAuthorization.Partial:
                    this.ConfigurePartial(provider);
                    break;
            }
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.Dispose(_authorization);
            switch (_authorization)
            {
                case GameAuthorization.Full:
                    this.DisposeFull();
                    break;
                case GameAuthorization.Partial:
                    this.DisposePartial();
                    break;
            }
        }

        protected virtual void ConfigureFull(ServiceProvider provider)
        {

        }

        protected virtual void ConfigurePartial(ServiceProvider provider)
        {

        }

        protected virtual void Configure(ServiceProvider provider, GameAuthorization authorization)
        {

        }

        protected virtual void DisposeFull()
        {

        }

        protected virtual void DisposePartial()
        {

        }

        protected virtual void Dispose(GameAuthorization authorization)
        {

        }
        #endregion
    }
}
