using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    public abstract class AetherEntity<T> : NetworkEntity
    {
        #region Private Fields
        /// <summary>
        /// The "live" instance. This is the master if there is no slave,
        /// otherwise it will be the slave.
        /// 
        /// This is used to grab the current state of the entity for rendering
        /// or informational purposes.
        /// </summary>
        private T _live;
        #endregion

        #region Protected Attributes
        /// <summary>
        /// The "real" instance of the current Farseer entity.
        /// 
        /// This should be changed to reflect rigid/instant states.
        /// </summary>
        protected internal T master { get; protected set; }

        /// <summary>
        /// The "visible" instance of the current Farseer entity.
        /// 
        /// This should only be updated internally via the UpdateSlave method or
        /// OnUpdateSlave event. This usually appears as a slow lerp towards the
        /// perfection of the master instance.
        /// </summary>
        protected internal T slave { get; protected set; }

        /// <summary>
        /// The "live" instance. This is the master if there is no slave,
        /// otherwise it will be the slave.
        /// 
        /// This is used to grab the current state of the entity for rendering
        /// or informational purposes.
        /// </summary>
        protected internal T live => _live;
        #endregion

        #region Events
        /// <summary>
        /// Event invoked during the Do method. This is used
        /// to update all internal farseer instances at once
        /// (master & slave if it exists). This should only
        /// be used for global instant changes that do not
        /// need to lerp.
        /// </summary>
        public event OnEventDelegate<AetherEntity<T>, Action<T>> OnDo;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Build(provider);
        }

        protected override void Release()
        {
            base.Release();

            this.OnDo -= AetherEntity<T>.DoMaster;
            this.OnDo -= AetherEntity<T>.DoSlave;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Apply an action to all internal
        /// Farseer objects.
        /// </summary>
        /// <param name="action"></param>
        public void Do(Action<T> action)
         => this.OnDo(this, action);

        /// <summary>
        /// Get a value from the live farseer object.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="getter"></param>
        /// <returns></returns>
        protected TOut Get<TOut>(Func<T, TOut> getter)
            => getter(_live);


        protected virtual void Build(ServiceProvider provider)
        {
            this.master = this.BuildMaster(provider);
            this.OnDo += AetherEntity<T>.DoMaster;
            _live = this.master;

            if (this.settings.Get<NetworkAuthorization>() < NetworkAuthorization.Master)
            {
                this.slave = this.BuildSlave(provider);
                this.OnDo += AetherEntity<T>.DoSlave;
                _live = this.slave;
            }
        }

        protected abstract T BuildMaster(ServiceProvider provider);
        protected abstract T BuildSlave(ServiceProvider provider);

        private static void DoMaster(AetherEntity<T> sender, Action<T> action)
            => action(sender.master);

        private static void DoSlave(AetherEntity<T> sender, Action<T> action)
            => action(sender.slave);
        #endregion
    }
}
