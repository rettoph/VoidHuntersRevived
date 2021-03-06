﻿using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities
{
    public abstract class AetherChildEntity<T, TParent> : AetherEntity<T>
    {
        #region Private Fields
        private Dictionary<T, TParent> _parents;
        private Dictionary<TParent, T> _children;
        #endregion

        #region Protected Fields
        protected AetherEntity<TParent> parent { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            _parents = new Dictionary<T, TParent>();
            _children = new Dictionary<TParent, T>();

            this.parent = this.GetParent(provider);
            this.parent.OnStatus[ServiceStatus.Releasing] += this.HandleParentReleased;

            base.PreInitialize(provider);
        }

        protected override void Release()
        {
            base.Release();

            this.parent.OnStatus[ServiceStatus.Releasing] -= this.HandleParentReleased;
            this.parent = null;
        }
        #endregion

        #region Helper Methods
        protected override T BuildMaster(ServiceProvider provider)
            => this.BuildWithParent(provider, this.parent.master);

        protected override T BuildSlave(ServiceProvider provider)
        => this.BuildWithParent(provider, this.parent.slave);

        protected T BuildWithParent(ServiceProvider provider, TParent parent)
        {
            var child = this.Build(provider, parent);

            _parents[child] = parent;
            _children[parent] = child;

            return child;
        }

        protected abstract T Build(ServiceProvider provider, TParent parent);
        protected abstract AetherEntity<TParent> GetParent(ServiceProvider provider);

        public TParent GetParent(T child)
            => _parents[child];

        public T GetChild(TParent parent)
            => _children[parent];
        #endregion

        #region Event Handlers
        private void HandleParentReleased(IService sender)
            => this.TryRelease();
        #endregion
    }
}
