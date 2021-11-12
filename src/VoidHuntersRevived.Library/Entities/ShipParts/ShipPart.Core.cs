using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Utilities;
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

            this.Tree_Release();
            this.Chain_Release();

            this.OnChainChanged -= this.HandleChainChanged;
            this.TryDestroyAetherForm();
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
        /// Attempt to build any aether world
        /// fixtures, bodies, joints, ect using
        /// the given Chain instance.
        /// </summary>
        /// <param name="chain"></param>
        protected virtual void TryCreateAetherForm(Chain chain)
        {
            // 
        }

        /// <summary>
        /// Attempt to destroy any existing physical 
        /// world fixtures, bodies, joints, ect.
        /// </summary>
        /// <param name="chain"></param>
        protected virtual void TryDestroyAetherForm()
        {
            // 
        }

        /// <summary>
        /// Update internal values to signify the Part's 
        /// coporeal status.
        /// </summary>
        /// <param name="corporeal"></param>
        protected virtual void TryUpdateCorporealState(Boolean corporeal)
        {
            // 
        }

        public virtual String ToAetherString()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Frame Methods
        public void TryDrawAt(GameTime gameTime, ref Matrix chainWorldTransformation)
        {
            Matrix worldTransformation = this.CalculateWorldTransformation(ref chainWorldTransformation);

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
                this.TryDestroyAetherForm();

                old.OnCorporealChanged -= this.HandleChainCorporealChanged;
            }
            

            if (value != default)
            {
                this.TryCreateAetherForm(value);
                this.TryUpdateCorporealState(value.Corporeal);

                value.OnCorporealChanged += this.HandleChainCorporealChanged;
            }
        }

        private void HandleChainCorporealChanged(AetherBodyWorldObject sender, bool corpreal)
        {
            this.TryUpdateCorporealState(corpreal);
        }
        #endregion
    }

    /// <summary>
    /// A ShipPart with a built in internal generic context.
    /// </summary>
    /// <typeparam name="TShipPartContext"></typeparam>
    public abstract class ShipPart<TShipPartContext> : ShipPart
        where TShipPartContext : ShipPartContext
    {
        /// <summary>
        /// The current ShipPart's context. This defines re-usable global
        /// ShipPart data.
        /// </summary>
        public new TShipPartContext Context { get; private set; }

        internal override void SetContext(ShipPartContext context)
        {
            ExceptionHelper.ValidateAssignableFrom<TShipPartContext>(context.GetType());

            base.SetContext(context);

            this.Context = context as TShipPartContext;
        }
    }
}
