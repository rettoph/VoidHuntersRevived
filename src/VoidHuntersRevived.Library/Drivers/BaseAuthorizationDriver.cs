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

            this.UpdateAuthorization(this.GetGameAuthorization());
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.Release(_authorization);
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

        protected virtual void ReleaseFull()
        {

        }

        protected virtual void ReleaseLocal()
        {

        }

        protected virtual void ReleaseMinimum()
        {

        }

        protected virtual void Release(GameAuthorization authorization)
        {

        }

        /// <summary>
        /// Both dispose previous cached authorization and
        /// setup the new value.
        /// </summary>
        /// <param name="authorization"></param>
        protected void UpdateAuthorization(GameAuthorization authorization)
        {
            this.DisposeAuthorization();
            this.ConfigureAuthorization(authorization);
        }

        protected void DisposeAuthorization()
        {
            this.Release(_authorization);

            if (_authorization.HasFlag(GameAuthorization.Full))
                this.ReleaseFull();
            if (_authorization.HasFlag(GameAuthorization.Local))
                this.ReleaseLocal();
            if (_authorization.HasFlag(GameAuthorization.Minimum))
                this.ReleaseMinimum();
        }

        protected void ConfigureAuthorization(GameAuthorization authorization)
        {
            _authorization = authorization;
            this.Configure(_provider, _authorization);
            if (_authorization.HasFlag(GameAuthorization.Full))
                this.ConfigureFull(_provider);
            if (_authorization.HasFlag(GameAuthorization.Local))
                this.ConfigureLocal(_provider);
            if (_authorization.HasFlag(GameAuthorization.Minimum))
                this.ConfigureMinimum(_provider);
        }

        protected virtual GameAuthorization GetGameAuthorization()
            => _provider.GetService<Settings>().Get<GameAuthorization>();
        #endregion
    }
}
