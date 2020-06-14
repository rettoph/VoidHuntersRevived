using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities
{
    public abstract class FarseerChildEntity<T, TParent> : FarseerEntity<T>
    {
        #region Protected Fields
        protected FarseerEntity<TParent> parent { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            this.parent = this.GetParent(provider);

            base.PreInitialize(provider);
        }
        #endregion

        #region Helper Methods
        protected override T BuildMaster(ServiceProvider provider)
            => this.Build(provider, this.parent.master);

        protected override T BuildSlave(ServiceProvider provider)
        => this.Build(provider, this.parent.slave);

        protected abstract T Build(ServiceProvider provider, TParent parent);
        protected abstract FarseerEntity<TParent> GetParent(ServiceProvider provider);
        #endregion
    }
}
