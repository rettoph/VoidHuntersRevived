using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Events.Delegates;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Interfaces;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    public abstract class ShipTractorBeamComponent : NetworkComponent<Ship>
    {
        #region Static Fields
        public static readonly Single MaxRange = Chunk.Size;
        private static readonly Vector2 MaxRangeVector = new Vector2(ShipTractorBeamComponent.MaxRange, 0);
        #endregion

        #region Private Fields
        private ShipTargetingComponent _targeting;
        private ChainService _chains;
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

            provider.Service(out _chains);
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            _chains = default;
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

            this.log.Info($"Attempted TractorBeam Action {action.Type} and recieved {response.Type}.");

            return response;
        }

        private TractorBeamAction TrySelect(TractorBeamAction action)
        {
            if (action.Type != TractorBeamActionType.Select)
                throw new ArgumentException($"Unable to create selection, Invalid ActionType({action.Type}) recieved.");

            if(this.CanSelect(action.TargetPart, out ShipPart selectable))
            {
                // Save the internal target part.
                _targeting = this.Entity.Components.Get<ShipTargetingComponent>();
                _targeting.OnTargetChanged += this.HandleShipTargetingComponentTargetChanged;

                this.Target = selectable.Chain;
                this.Target.OnStatusChanged += this.HandleTargetStatusChanged;

                return new TractorBeamAction(
                    type: TractorBeamActionType.Select,
                    targetShipPart: selectable);
            }

            return default;
        }

        private TractorBeamAction TryDeselect(TractorBeamAction action)
        {
            if (!action.Type.HasFlag(TractorBeamActionType.Deselect))
            {
                throw new ArgumentException($"Unable to create deselection, Invalid ActionType({action.Type}) recieved.");
            }

            if(this.CanDeselect(action.TargetPart))
            {
                ConnectionNode childNode = this.Target.Root.ConnectionNodes.FirstOrDefault();

                this.Target.OnStatusChanged -= this.HandleTargetStatusChanged;
                this.Target = default;

                _targeting.OnTargetChanged -= this.HandleShipTargetingComponentTargetChanged;
                _targeting = default;

                if (action.Type.HasFlag(TractorBeamActionType.Attach))
                {
                    return this.TryAttach(action, childNode);
                }

                return new TractorBeamAction(
                    type: TractorBeamActionType.Deselect);
            }

            return default;
        }

        private TractorBeamAction TryAttach(TractorBeamAction action, ConnectionNode child)
        {
            if(this.CanAttach(action.TargetNode, child))
            {
                action.TargetNode.TryAttach(child);
            }

            return action;
        }

        /// <summary>
        /// Determin whether or not a part can be selected.
        /// If needed the returned output will contain mutated data
        /// to ensure the root piece is always selected unless its an internal
        /// chain.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="selectable"></param>
        /// <returns></returns>
        private Boolean CanSelect(ShipPart target, out ShipPart selectable)
        {
            Boolean FailureResponse(out ShipPart selectable)
            {
                selectable = default;
                return false;
            }

            if(this.Target != default)
            {
                return FailureResponse(out selectable);
            }

            if (target?.Chain == default)
            {
                return FailureResponse(out selectable);
            }

            if (!target.Chain.Corporeal)
            {
                selectable = target.Root;
                return true;
            }

            if(target.Chain == this.Entity.Chain && !target.IsRoot)
            {
                // If this condition is met we are attempting to disconnect a piece from the current ship. That process is done here.
                target.ChildConnectionNode?.TryDetach();
                _chains.Create(target);

                selectable = target;
                return true;
            }


            return FailureResponse(out selectable); ;
        }

        private Boolean CanDeselect(ShipPart target)
        {
            if (this.Target == default)
                return false;

            return true;
        }

        private Boolean CanAttach(ConnectionNode parent, ConnectionNode child)
        {
            if(parent?.Connection.State != ConnectionNodeState.Estranged)
            {
                return false;
            }

            if (child?.Connection.State != ConnectionNodeState.Estranged)
            {
                return false;
            }

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

        private void HandleTargetStatusChanged(IService sender, ServiceStatus old, ServiceStatus value)
        {
            if(value == ServiceStatus.PreReleasing)
            {
                this.TryAction(new TractorBeamAction(type: TractorBeamActionType.Deselect));
            }
        }
        #endregion
    }
}
