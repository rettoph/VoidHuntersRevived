using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public abstract partial class ShipPart : Entity
    {
        #region Public Properties
        /// <summary>
        /// The current ShipPart's context. This defines re-usable global
        /// ShipPart data.
        /// </summary>
        public ShipPartContext Context { get; private set; }
        #endregion

        #region Events
        public delegate void OnDrawAtDelegate(GameTime gameTime, ref Matrix worldTransformation);

        public OnDrawAtDelegate OnDrawAt;
        #endregion

        #region Lifecycle Methods
        protected override void Create(GuppyServiceProvider provider)
        {
            base.Create(provider);

            this.Tree_Create(provider);
            this.Transformations_Create(provider);
            this.Chain_Create(provider);
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.Tree_Initialize(provider);

            this.OnChainChanged += this.HandleChainChanged;
        }

        protected override void PostInitialize(GuppyServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.Tree_PostInitialize(provider);
        }

        protected override void Release()
        {
            base.Release();

            this.Transformations_Dispose();
            this.Tree_Release();

            this.OnChainChanged -= this.HandleChainChanged;
            this.TryDestroyCorporealForm();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.Chain_Dispose();
            this.Transformations_Dispose();
            this.Tree_Dispose();
        }
        #endregion

        #region Helper Methods
        internal virtual void SetContext(ShipPartContext context)
            => this.Context = context;

        /// <summary>
        /// Attempt to build any physical world
        /// fixtures, bodies, joints, ect using
        /// the given Chain instance.
        /// </summary>
        /// <param name="chain"></param>
        protected virtual void TryCreateCorporealForm(Chain chain)
        {

        }

        /// <summary>
        /// Attempt to destroy any existing physical 
        /// world fixtures, bodies, joints, ect.
        /// </summary>
        /// <param name="chain"></param>
        protected virtual void TryDestroyCorporealForm()
        {

        }
        #endregion

        #region Frame Methods
        public void TryDrawAt(GameTime gameTime, ref Matrix chainWorldTransformation)
        {
            Matrix worldTransformation = this.LocalTransformation * chainWorldTransformation;

            this.OnDrawAt.Invoke(gameTime, ref worldTransformation);

            foreach (ConnectionNode node in this.ConnectionNodes)
                if (node.Connection.State == ConnectionNodeState.Parent)
                    node.Connection.Target.Owner.TryDrawAt(gameTime, ref chainWorldTransformation);
        }
        #endregion

        #region Event Handlers
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if(old != default)
            {
                if(old.Corporeal)
                    this.TryDestroyCorporealForm();

                old.OnCorporealChanged -= this.HandleChainCorporealChanged;
            }
            

            if (value != default)
            {
                if(value.Corporeal)
                    this.TryCreateCorporealForm(value);

                value.OnCorporealChanged += this.HandleChainCorporealChanged;
            }
        }

        private void HandleChainCorporealChanged(AetherBodyWorldObject sender, bool corpreal)
        {
            if(corpreal)
            {
                this.TryCreateCorporealForm(sender as Chain);
            }
            else
            {
                this.TryDestroyCorporealForm();
            }
        }
        #endregion
    }
}
