using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Enums;
using Guppy.EntityComponent.Interfaces;
using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using Guppy.Threading.Utilities;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.MessageProcessors;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Messages.Requests;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Components.Ships
{
    public sealed class TractorBeamComponent : ReferenceComponent<Ship, TargetComponent>,
        IDataProcessor<TractorBeamStateRequest>
    {
        public const Single AABBQuerySize = 3;

        #region Private Fields
        private TractorBeamState _state;
        private TractorBeamRequestProcessor _requestProcessor;
        private AetherWorld _world;
        private ChainService _chains;
        private ILogger _logger;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current selected target, if any.
        /// </summary>
        public TractorBeamState State
        {
            get => _state;
            private set
            {
                this.OnStateChanged.InvokeIf(_state != value, this, ref _state, value);
            }
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<TractorBeamComponent, TractorBeamState> OnStateChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _requestProcessor);
            provider.Service(out _world);
            provider.Service(out _chains);
            provider.Service(out _logger);

            this.State = TractorBeamState.Default;
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            _requestProcessor.TryAdd(this);
        }

        protected override void PreUninitialize()
        {
            base.PreUninitialize();

            _requestProcessor.Remove(this);
        }
        #endregion

        #region Frame Methods
        private void UpdateTarget(GameTime gameTime)
        {
            ConnectionNode potentialParent = this.GetMostViableConnectionNodeTarget();

            this.SetTargetPosition(this.State.TargetPart, potentialParent);
        }

        private void SetTargetPosition(ShipPart target, ConnectionNode destination)
        {
            if (this.Entity.Chain is null)
            {
                _logger.Warning($"{nameof(TractorBeamComponent)}::{nameof(SetTargetPosition)} - {nameof(this.Entity)}.{nameof(this.Entity.Chain)} is null. Unable to update position.");
                this.EnqueueDeselect();

                return;
            }
            
            if (target is null)
            {
                _logger.Warning($"{nameof(TractorBeamComponent)}::{nameof(SetTargetPosition)} - {nameof(target)} is null. Unable to update position.");
                this.EnqueueDeselect();

                return;
            }
            
            if (target.Chain is null)
            {
                _logger.Warning($"{nameof(TractorBeamComponent)}::{nameof(SetTargetPosition)} - {nameof(target)}.{nameof(target.Chain)} is null. Unable to update position.");
                this.EnqueueDeselect();

                return;
            }

            if (destination is null)
            {
                target.Chain.Body.SetTransformIgnoreContacts(this.Reference.Value, target.Chain.Rotation);
                return;
            }

            this.SetPreviewPosition(target, destination);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Return the closest (if any) <see cref="ShipPart"/> that may be selected
        /// based on the <see cref="TargetComponent.Value"/> position.
        /// </summary>
        /// <returns></returns>
        public ShipPart GetMostViableShipPartTarget()
        {
            AABB aabb = new AABB(this.Reference.Value, AABBQuerySize, AABBQuerySize);
            ShipPart target = default;
            Single targetDistance = Single.MaxValue, distance;

            _world.LocalInstance.QueryAABB(fixture =>
            {
                if (fixture.Tag is ShipPart shipPart)
                {
                    if (shipPart.Chain == this.Entity.Chain && shipPart.IsRoot)
                    {
                        return true;
                    }

                    if(shipPart.Chain != this.Entity.Chain && shipPart.Chain.Corporeal)
                    {
                        return true;
                    }

                    Vector2 worldCenteroid = shipPart.CalculateWorldPoint(shipPart.Context.Centeroid);

                    distance = Vector2.Distance(this.Reference.Value, worldCenteroid);

                    if (distance < targetDistance)
                    {
                        targetDistance = distance;

                        if(shipPart.Chain == this.Entity.Chain)
                        {
                            target = shipPart;
                        }
                        else
                        {
                            target = shipPart.Root;
                        }
                    }
                }

                return true;
            }, ref aabb);

            return target;
        }

        public ConnectionNode GetMostViableConnectionNodeTarget()
        {
            ConnectionNode closestEstrangedNode = this.Entity.Chain.Root
                .GetChildren()
                .SelectMany(sp => sp.ConnectionNodes)
                .Where(cn => cn.Connection.State == ConnectionNodeState.Estranged)
                .Where(cn => Vector2.Distance(this.Reference.Value, cn.Owner.CalculateWorldPoint(cn.LocalPosition)) < 1f)
                .OrderBy(cn => Vector2.Distance(this.Reference.Value, cn.Owner.CalculateWorldPoint(cn.LocalPosition)))
                .FirstOrDefault();

            return closestEstrangedNode;
        }

        /// <summary>
        /// Calculate where the recieved target *would be* were it connected 
        /// to the destination node.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        private void GetPreviewPosition(ShipPart target, ConnectionNode parent, out Vector2 position, out Single rotation)
        {
            rotation = ShipPart.CalculateLocalRotation(
                child: target.ConnectionNodes[0],
                parent: parent) + this.Entity.Chain.Rotation;

            Matrix potentialTransformation = ShipPart.CalculateLocalTransformation(
                child: target.ConnectionNodes[0],
                parent: parent) * this.Entity.Chain.WorldTransformation;

            position = Vector2.Transform(Vector2.Zero, potentialTransformation);
        }

        private void SetPreviewPosition(ShipPart target, ConnectionNode parent)
        {
            this.GetPreviewPosition(target, parent, out Vector2 position, out Single rotation);

            target.Chain.Body.SetTransformIgnoreContacts(position, rotation);
        }

        /// <summary>
        /// Enqueue a <see cref="TractorBeamStateRequest"/> to be preformed next time the <see cref="Globals.Constants.MessageQueues.TractorBeamRequestQueue"/> queue is flushed.
        /// </summary>
        /// <param name="type">The specific <see cref="TractorBeamStateType"/> to be performed.</param>
        /// <param name="targetPart">The target <see cref="ShipPart"/> in question the <paramref name="type"/> is to be preformed on.</param>
        /// <param name="destinationNode">
        /// The <see cref="ConnectionNode"/>, if any, the <see cref="TractorBeamStateType"/> is to be
        /// preformed on. This is generally used to defined which node
        /// the <see cref="ShipPart"/> wishes to attach to when
        /// the <paramref name="type"/> is <see cref="TractorBeamStateType.Deselect"/>.
        /// </param>
        public void EnqueueState(HostType requestHost, TractorBeamStateType type, ShipPart targetPart, ConnectionNode destinationNode)
        {
            _requestProcessor.Enqueue(new TractorBeamStateRequest()
            {
                Id = this.Id,
                Type = type,
                TargetPart = targetPart,
                DestinationNode = destinationNode,
                RequestHost = requestHost
            });
        }

        public void EnqueueSelect()
        {
            this.EnqueueState(
                HostType.Local,
                TractorBeamStateType.Select,
                this.GetMostViableShipPartTarget(),
                default);
        }

        public void EnqueueDeselect()
        {
            this.EnqueueState(
                HostType.Local,
                TractorBeamStateType.Deselect,
                this.State.TargetPart,
                this.GetMostViableConnectionNodeTarget());
        }
        #endregion

        #region Message Processors
        bool IDataProcessor<TractorBeamStateRequest>.Process(TractorBeamStateRequest request)
        {
            TractorBeamState response = request.Type switch
            {
                TractorBeamStateType.Select => this.ProcessSelect(request),
                TractorBeamStateType.Deselect => this.ProcessDeselect(request),
                _ => TractorBeamState.Default
            };

            _logger.Verbose("{class}::{method} - Recieved {type} {request} from {host} host and got {responseType} back.\nOld State Hash: {oldHash}\nNew State Hash: {newHash}", 
                nameof(TractorBeamComponent), 
                nameof(IDataProcessor<TractorBeamStateRequest>.Process), 
                request.Type, 
                nameof(TractorBeamStateRequest), 
                request.RequestHost,
                response.Type,
                this.State.GetHashCode(),
                response.GetHashCode());

            this.State = response;

            return true;
        }

        private TractorBeamState ProcessSelect(TractorBeamStateRequest request)
        {
            // Already selecting somthing...
            if (this.State.Type == TractorBeamStateType.Select)
            {
                return this.State;
            }

            // Request target is null...
            if (request.TargetPart?.Chain is null)
            {
                return this.State;
            }

            // Is current bridge...
            if(request.TargetPart.Chain.Id == this.Entity.Chain.Id
                && request.TargetPart.IsRoot)
            {
                return this.State;
            }

            // Attached to another ship...
            if (request.TargetPart.Chain.Corporeal
                && request.TargetPart.Chain.Id != this.Entity.Chain.Id)
            {
                return this.State;
            }

            // Ensure the part gets detached from its chain...
            if(request.TargetPart.ChildConnectionNode is not null)
            {
                this.GetPreviewPosition(
                    request.TargetPart,
                    request.TargetPart.ChildConnectionNode.Connection.Target, out Vector2 position, out Single rotation);
                request.TargetPart.ChildConnectionNode?.TryDetach();
                _chains.Create(request.TargetPart, position, rotation);
            }

            this.Entity.OnPostUpdate += this.UpdateTarget;

            return TractorBeamState.Select(request.TargetPart, request.RequestHost);
        }


        private TractorBeamState ProcessDeselect(TractorBeamStateRequest request)
        {
            // Request target is null...
            if (request.TargetPart?.Chain is null)
            {
                return this.State;
            }

            // Request or destination nodes are not estranged
            if (request.DestinationNode is not null
                && request.DestinationNode.Connection.State != ConnectionNodeState.Estranged
                && request.TargetPart.ConnectionNodes.FirstOrDefault()?.Connection.State != ConnectionNodeState.Estranged)
            {
                return this.State;
            }

            request.DestinationNode?.TryAttach(request.TargetPart.ConnectionNodes.FirstOrDefault());

            this.Entity.OnPostUpdate -= this.UpdateTarget;

            return TractorBeamState.Deselect(request.TargetPart, request.DestinationNode, request.RequestHost);
        }
        #endregion
    }
}
