using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Events.Delegates;
using Guppy.Extensions.log4net;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Interfaces;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Threading.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision;
using VoidHuntersRevived.Library.Entities.Aether;
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
        private AetherWorld _world;
        private ThreadQueue _mainThread;
        private Single _rotation;
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
        public ShipPart Target { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            _targeting = this.Entity.Components.Get<ShipTargetingComponent>();

            provider.Service(out _chains);
            provider.Service(out _world);
            provider.Service(out _mainThread);

            _targeting.OnTargetChanged += this.HandleShipTargetChanged;
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            _targeting.OnTargetChanged -= this.HandleShipTargetChanged;

            _targeting = default;
            _chains = default;
            _world = default;
            _mainThread = default;
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
        private void UpdateTarget(GameTime gameTime)
        {
            ConnectionNode potentialParent = this.GetConnectionNodeTarget(this.Position);

            this.SetTargetPosition(this.Target, potentialParent);
        }
        #endregion

        #region Events
        public OnEventDelegate<ShipTractorBeamComponent, TractorBeamAction> OnAction;
        #endregion

        #region Methods
        private void GetPreviewPosition(ShipPart target, ConnectionNode parent, out Vector2 position, out Single rotation)
        {
            rotation = ShipPart.CalculateLocalRotation(
                child: target.ConnectionNodes[0],
                parent: parent) + this.Entity.Chain.Rotation;

            Matrix potentialTransformation = ShipPart.CalculateLocalTransformation(
                child: target.ConnectionNodes[0],
                parent: parent) * this.Entity.Chain.CalculateWorldTransformation();

            position = Vector2.Transform(Vector2.Zero, potentialTransformation);
        }

        private void SetPreviewPosition(ShipPart target, ConnectionNode parent)
        {
            this.GetPreviewPosition(target, parent, out Vector2 position, out Single rotation);

            target.Chain.Body.SetTransformIgnoreContacts(position, rotation);
        }

        private void SetTargetPosition(ShipPart target, ConnectionNode potentialParent)
        {
            if (this.Entity.Chain is null)
            {
                this.log.Warn($"{nameof(ShipTractorBeamComponent)}::{nameof(SetTargetPosition)} - {nameof(this.Entity)}.{nameof(this.Entity.Chain)} is null. Unable to update position.");

                return;
            }
            else if (target is null)
            {
                this.log.Warn($"{nameof(ShipTractorBeamComponent)}::{nameof(SetTargetPosition)} - {nameof(target)} is null. Unable to update position.");

                return;
            }
            else if (target.Chain is null)
            {
                this.log.Warn($"{nameof(ShipTractorBeamComponent)}::{nameof(SetTargetPosition)} - {nameof(target)}.{nameof(target.Chain)} is null. Unable to update position.");

                return;
            }

            if (potentialParent is null)
            {
                target.Chain.Body.SetTransformIgnoreContacts(this.Position, target.Chain.Rotation);
            }
            else
            {
                this.SetPreviewPosition(target, potentialParent);
            }
        }

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

            this.log.Verbose($"Attempted TractorBeam Action {action.Type} and recieved {response.Type}.");

            return response;
        }

        private TractorBeamAction TrySelect(TractorBeamAction action)
        {
            if (action.Type != TractorBeamActionType.Select)
                throw new ArgumentException($"Unable to create selection, Invalid ActionType({action.Type}) recieved.");

            switch (this.CanSelect(action.TargetPart, out ShipPart selectable))
            {
                case CanSelectResponse.No:
                    return default;
                case CanSelectResponse.Yes:
                    return this.Select(selectable);
                case CanSelectResponse.YesAfterDetach:
                    this.GetPreviewPosition(selectable, selectable.ChildConnectionNode.Connection.Target, out Vector2 position, out Single rotation);
                    selectable.ChildConnectionNode.TryDetach();
                    _chains.Create(selectable, position, rotation, action.TargetPartChainId);

                    return this.Select(selectable);
            }

            return default;
        }

        private TractorBeamAction Select(ShipPart target)
        {
            _rotation = 0;
            this.Target = target;
            this.Target.OnStatusChanged += this.HandleTargetStatusChanged;

            this.Entity.OnUpdate += this.UpdateTarget;

            return new TractorBeamAction(
                type: TractorBeamActionType.Select,
                targetShipPart: target,
                targetShipPartChainId: target.Chain?.Id);
        }

        private TractorBeamAction TryDeselect(TractorBeamAction action)
        {
            if (!action.Type.HasFlag(TractorBeamActionType.Deselect))
            {
                throw new ArgumentException($"Unable to create deselection, Invalid ActionType({action.Type}) recieved.");
            }

            if(this.CanDeselect(action.TargetPart))
            {
                ConnectionNode childNode = this.Target.Root.ConnectionNodes.ElementAtOrDefault(0);

                this.Entity.OnUpdate -= this.UpdateTarget;

                this.SetTargetPosition(this.Target, default);

                this.Target.OnStatusChanged -= this.HandleTargetStatusChanged;
                this.Target = default;

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
        private CanSelectResponse CanSelect(ShipPart target, out ShipPart selectable)
        {
            CanSelectResponse FailureResponse(out ShipPart selectable)
            {
                selectable = default;
                return CanSelectResponse.No;
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
                return CanSelectResponse.Yes;
            }

            if(target.Chain == this.Entity.Chain && !target.IsRoot)
            {
                selectable = target;
                return CanSelectResponse.YesAfterDetach;
            }

            return FailureResponse(out selectable);
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

        public ShipPart GetShipPartTarget(Vector2? position = default)
        {
            position ??= this.Position;

            // Sweep the world for any valid/selectable ShipParts.
            AABB aabb = new AABB(
                min: position.Value - (Vector2.One * 2.5f),
                max: position.Value + (Vector2.One * 2.5f));
            ShipPart target = default;
            Single targetDistance = Single.MaxValue, distance;

            _world.LocalInstance.QueryAABB(fixture =>
            {
                if (fixture.Tag is ShipPart shipPart)
                {
                    Vector2 worldCenteroid = shipPart.CalculateWorldPoint(shipPart.Context.Centeroid);

                    distance = Vector2.Distance(position.Value, worldCenteroid);

                    if (distance < targetDistance)
                    {
                        targetDistance = distance;
                        target = shipPart;
                    }
                }

                return true;
            }, ref aabb);

            return target;
        }

        public ConnectionNode GetConnectionNodeTarget(Vector2? position = default)
        {
            position ??= this.Position;

            ConnectionNode closestEstrangedNode = this.Entity.Chain.Root
                .GetChildren()
                .SelectMany(sp => sp.ConnectionNodes)
                .Where(cn => cn.Connection.State == ConnectionNodeState.Estranged)
                .Where(cn => Vector2.Distance(position.Value, cn.Owner.CalculateWorldPoint(cn.LocalPosition)) < 1f)
                .OrderBy(cn => Vector2.Distance(position.Value, cn.Owner.CalculateWorldPoint(cn.LocalPosition)))
                .FirstOrDefault();

            return closestEstrangedNode;
        }
        #endregion

        #region Event Handlers
        private void HandleShipTargetChanged(Ship sender, Vector2 target)
        {
            this.Position = this.GetValidTractorbeamPosition(target);
        }

        private void HandleTargetStatusChanged(IService sender, ServiceStatus old, ServiceStatus value)
        {
            if(value == ServiceStatus.PreReleasing && sender is Chain oldChain)
            {
                _mainThread.Enqueue(gt =>
                {
                    this.log.Info($"{nameof(ShipTractorBeamComponent)}::{nameof(HandleTargetStatusChanged)} => TractorBeam Target is releasing.");
                    this.TryAction(new TractorBeamAction(type: TractorBeamActionType.Deselect));
                });
            }
        }
        #endregion
    }
}
