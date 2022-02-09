using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.MessageProcessors;
using VoidHuntersRevived.Library.Messages.Requests;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Ships
{
    public sealed class DirectionComponent : ShipShipPartsComponent<Thruster, DirectionRequest, DirectionRequestProcessor, DirectionComponent>
    {
        #region Public Attributes
        /// <summary>
        /// The current Ship's active directions. This can be updated via 
        /// the <see cref="TrySetDirection(Direction, bool)"/> helper method.
        /// </summary>
        public Direction ActiveDirections { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Event invoked when the <see cref="ActiveDirections"/> property value
        /// is updated via the <see cref="TrySetDirection(DirectionRequest)"/> method.
        /// 
        /// This will contain the changed direction and its new state.
        /// </summary>
        public event OnEventDelegate<DirectionComponent, DirectionRequest> OnDirectionChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.OnShipPartsUpdated += this.HandleShipPartsUpdated;
            this.OnDirectionChanged += this.HandleDirectionChanged;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.OnShipPartsUpdated -= this.HandleShipPartsUpdated;
            this.OnDirectionChanged -= this.HandleDirectionChanged;
        }
        #endregion

        #region Helper Methods
        private void CleanPoweredThrusters()
        {
            foreach(Thruster thruster in this.ShipParts)
            {
                thruster.UpdatePowered(this.ActiveDirections);
            }
        }

        public void EnqueueRequest(Direction direction, Boolean state, HostType requestHost)
        {
            this.consolidationProcessor.Enqueue(new DirectionRequest()
            {
                Id = this.Id,
                Direction = direction,
                State = state,
                RequestHost = requestHost
            });
        }

        /// <summary>
        /// Attempt to set a directional flag within the current ship.
        /// If the change goes through, the <see cref="OnDirectionChanged"/>
        /// event will be invoked.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Boolean Process(DirectionRequest request)
        {
            if ((request.Direction & (request.Direction - 1)) != 0)
                throw new Exception("Unable to set multiple directions at once.");

            if (request.State && (this.ActiveDirections & request.Direction) == 0)
                this.ActiveDirections |= request.Direction;
            else if (!request.State && (this.ActiveDirections & request.Direction) != 0)
                this.ActiveDirections &= ~request.Direction;
            else
                return false;

            // If we've made it this far then we know that the directional change was a success.
            // Invoke the direction changed event now.
            this.OnDirectionChanged?.Invoke(this, request);

            return true;
        }
        #endregion

        private void HandleDirectionChanged(DirectionComponent sender, DirectionRequest args)
        {
            this.CleanPoweredThrusters();
        }

        private void HandleShipPartsUpdated(IEnumerable<Thruster> args)
        {
            this.CleanPoweredThrusters();
        }
    }
}
