using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    public abstract class ShipTractorBeamComponent : NetworkComponent<Ship>
    {
        #region Static Fields
        public static readonly Single MaxRange = Chunk.Size;
        private static readonly Vector2 MaxRangeVector = new Vector2(ShipTractorBeamComponent.MaxRange, 0);
        #endregion

        #region Protected Properties
        protected ShipPartService shipParts { get; private set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// The Ship's TractorBeam Target differes from <see cref="ShipTargetingComponent.Target"/>
        /// because TractorBeam has a <see cref="MaxRange"/>.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// The current TractorBeam's target, if any.
        /// </summary>
        public Chain Target { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);
        }

        protected override void PostRelease()
        {
            base.PostRelease();
        }

        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.shipParts = provider.GetService<ShipPartService>();

            this.Entity.Messages.Add(Constants.Messages.Ship.TractorBeamAction, Guppy.Network.Constants.MessageContexts.InternalUnreliableDefault);
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.shipParts = default;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Events
        public OnEventDelegate<ShipTractorBeamComponent, TractorBeamAction> OnAction;
        #endregion

        #region Methods
        public TractorBeamAction TryAction(TractorBeamAction action)
        {
            TractorBeamAction response = action.Type switch
            {
                TractorBeamActionType.Select => this.TrySelect(action),
                TractorBeamActionType.Attach => this.TryDeselect(action),
                TractorBeamActionType.Deselect => this.TryDeselect(action),
                _ => action
            };

            if(action.Type != TractorBeamActionType.None)
                this.OnAction?.Invoke(this, action);

            return response;
        }

        private TractorBeamAction TrySelect(TractorBeamAction action)
        {
            if (action.Type != TractorBeamActionType.Select)
                throw new ArgumentException($"Unable to create selection, Invalid ActionType({action.Type}) recieved.");

            if(this.CanSelect(action.TargetPart))
            {
                // Ensure the part is detached before doing anything.
                action.TargetPart.ChildConnectionNode?.TryDetach();

                // Save the internal target part.
                this.Target = action.TargetPart.Chain;
                this.Entity.Components.Get<ShipTargetingComponent>().OnTargetChanged += this.HandleShipTargetingComponentTargetChanged;

                return action;
            }

            return default;
        }

        private TractorBeamAction TryDeselect(TractorBeamAction action)
        {
            if (!action.Type.HasFlag(TractorBeamActionType.Deselect))
                throw new ArgumentException($"Unable to create deselection, Invalid ActionType({action.Type}) recieved.");

            this.Entity.Components.Get<ShipTargetingComponent>().OnTargetChanged -= this.HandleShipTargetingComponentTargetChanged;
            this.Target = default;

            if (action.Type.HasFlag(TractorBeamActionType.Attach))
            {
                return this.TryAttach(action);
            }

            return action;
        }

        private TractorBeamAction TryAttach(TractorBeamAction action)
        {
            // throw new NotImplementedException();
            return action;
        }

        private Boolean CanSelect(ShipPart target)
        {
            if (this.Target != default)
                return false;

            if (target?.Chain == default)
                return false;

            if (target.Chain.Corporeal && this.Entity.Chain != target.Chain)
                return false;

            return true;
        }

        public Vector2 GetValidTractorbeamPosition(Vector2 worldPosition)
        {
            Single distance = Vector2.Distance(worldPosition, this.Entity.Chain.Position);

            if (distance <= ShipTractorBeamComponent.MaxRange)
            {
                return worldPosition;
            }
            else
            {
                Single angle = worldPosition.Angle(this.Entity.Chain.Position);
                return this.Entity.Chain.Position + ShipTractorBeamComponent.MaxRangeVector.RotateTo(angle);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleShipTargetingComponentTargetChanged(Ship sender, Vector2 target)
        {
            if (this.Entity.Chain == default)
                return;

            this.Position = this.GetValidTractorbeamPosition(target);
            this.Target.Body.SetTransformIgnoreContacts(this.Position, 0);
        }
        #endregion
    }
}
