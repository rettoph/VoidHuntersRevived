using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Drivers
{
    public class BaseAuthorizationDriver<TDriven> : Driver<TDriven>
        where TDriven : Driven
    {
        #region Private Fields
        private GameAuthorization _authorization;
        private ServiceProvider _provider;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            _provider = provider;

            this.UpdateAuthorization(this.GetDefaultAuthorization());
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.Dispose(_authorization);
            this.UpdateAuthorization(GameAuthorization.None);
        }

        protected virtual void ConfigureFull(ServiceProvider provider)
        {

        }

        protected virtual void ConfigureLocal(ServiceProvider provider)
        {

        }

        protected virtual void ConfigureMinimum(ServiceProvider provider)
        {

        }

        protected virtual void Configure(ServiceProvider provider, GameAuthorization authorization)
        {

        }

        protected virtual void DisposeFull()
        {

        }

        protected virtual void DisposeLocal()
        {

        }

        protected virtual void DisposeMinimum()
        {

        }

        protected virtual void Dispose(GameAuthorization authorization)
        {

        }

        protected void UpdateAuthorization(GameAuthorization authorization)
        {
            this.Dispose(authorization);

            switch (_authorization)
            {
                case GameAuthorization.Full:
                    this.DisposeFull();
                    break;
                case GameAuthorization.Local:
                    this.DisposeLocal();
                    break;
                case GameAuthorization.Minimum:
                    this.DisposeMinimum();
                    break;
            }

            _authorization = authorization;
            this.Configure(_provider, _authorization);
            switch (_authorization)
            {
                case GameAuthorization.Full:
                    this.ConfigureFull(_provider);
                    break;
                case GameAuthorization.Local:
                    this.ConfigureLocal(_provider);
                    break;
                case GameAuthorization.Minimum:
                    this.ConfigureMinimum(_provider);
                    break;
            }
        }

        protected virtual GameAuthorization GetDefaultAuthorization()
            => _provider.GetService<Settings>().Get<GameAuthorization>();
        #endregion
    }
}
