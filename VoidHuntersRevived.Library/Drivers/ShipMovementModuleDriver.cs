using Guppy.Implementations;
using Guppy.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers
{
    /// <summary>
    /// Inherently, ships do not contain any movement
    /// functionality. This driver demonstrates the ability
    /// to add custom functionality to a pre-existing class 
    /// through a driver and the custom event invoker
    /// </summary>
    public class ShipMovementModuleDriver : Driver
    {
        private Ship _ship;
        private Dictionary<Direction, Boolean> _directions;
        private List<Thruster> _thrusters;
        private Dictionary<Direction, List<Thruster>> _thrusterDirections;

        public ShipMovementModuleDriver(Ship parent, IServiceProvider provider) : base(parent, provider)
        {
            _ship = parent;
        }

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            // Add ship event listeners
            _ship.Disposing += this.HandleShipDisposing;
            _ship.Events.AddHandler("updated:children", this.HandleShipUpdatedChildren);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _directions = (Enum.GetValues(typeof(Direction)) as Direction[]).ToDictionary(d => d, d => false);
            _thrusters = new List<Thruster>();
            _thrusterDirections = (Enum.GetValues(typeof(Direction)) as Direction[])
                .ToDictionary(
                    keySelector: d => d,
                    elementSelector: d => new List<Thruster>());
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (_ship.Bridge != null)
            { // We only need to bother moving the ship if there is a bridge defined...
                _ship.Bridge.AngularDamping = MathHelper.Lerp(_ship.Bridge.AngularDamping, (_directions[Direction.TurnLeft] || _directions[Direction.TurnRight]) ? 1f : 1.5f, 0.25f);

                foreach (Thruster thruster in _thrusters)
                    thruster.SetActive(false);

                foreach (KeyValuePair<Direction, Boolean> kvp in _directions)
                    if (kvp.Value)
                        foreach (Thruster thruster in _thrusterDirections[kvp.Key])
                            thruster.SetActive(true);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Set one of the ships directions
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        public void SetDirection(Direction direction, Boolean value)
        {
            if (_directions[direction] != value)
            {
                this.logger.LogDebug($"Setting Ship({_ship.Id}) Direction<{direction}> to {value}...");

                _directions[direction] = value;

                _ship.Events.TryInvoke("changed:direction", direction);
            }
        }

        /// <summary>
        /// Get the ships current direction value
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Boolean GetDirection(Direction direction)
        {
            return _directions[direction];
        }
        #endregion

        #region Network Methods
        public void ReadDirectionData(NetIncomingMessage im)
        {
            this.SetDirection(
                (Direction)im.ReadByte(),
                im.ReadBoolean());
        }

        public void WriteDirectionData(NetOutgoingMessage om, Direction direction)
        {
            om.Write((Byte)direction);
            om.Write(_directions[direction]);
        }
        #endregion

        #region Events
        private void HandleShipDisposing(object sender, ITrackedDisposable e)
        {
            // Remove event listeners
            _ship.Disposing -= this.HandleShipDisposing;
            _ship.Events.RemoveHandler("updated:children", this.HandleShipUpdatedChildren);
        }

        /// <summary>
        /// When the parent ships children are updated, we must remap 
        /// the internal thruster settings...
        /// </summary>
        /// <param name="arg"></param>
        private void HandleShipUpdatedChildren(Object sender, Object arg)
        {
            var children = arg as List<ShipPart>;

            // First ensure that every old thruster is turned off
            foreach (Thruster thruster in _thrusters)
                thruster.SetActive(false);

            // Clear the old thruster list
            _thrusters.Clear();
            _thrusters.AddRange(children
                .Where(c => typeof(Thruster).IsAssignableFrom(c.GetType()))
                .Select(c => c as Thruster));

            // Clear the old thruster mapping...
            foreach (Direction direction in Enum.GetValues(typeof(Direction)) as Direction[])
                _thrusterDirections[direction].Clear();

            // Remap the thruster data
            var directions = new List<Direction>();
            foreach (Thruster thruster in _thrusters)
                foreach (Direction direction in thruster.GetDirections(ref directions))
                    _thrusterDirections[direction].Add(thruster);
        }
        #endregion
    }

    /// <summary>
    /// This extension class demonstrates how we can interface directly into
    /// the ship without editing the ship file directly.
    /// </summary>
    public static class ShipMovementExtensions {
        #region Wrapper Methods
        /// <summary>
        /// Set one of the ships directions
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        public static void SetDirection(this Ship ship, Direction direction, Boolean value)
        {
            ship.GetDriver<ShipMovementModuleDriver>().SetDirection(direction, value);
        }

        /// <summary>
        /// Get the ships current direction value
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Boolean GetDirection(this Ship ship, Direction direction)
        {
            return ship.GetDriver<ShipMovementModuleDriver>().GetDirection(direction);
        }
        #endregion

        #region Network Methods
        public static void ReadDirectionData(this Ship ship, NetIncomingMessage im)
        {
            ship.GetDriver<ShipMovementModuleDriver>().ReadDirectionData(im);
        }

        public static void WriteDirectionData(this Ship ship, NetOutgoingMessage om, Direction direction)
        {
            ship.GetDriver<ShipMovementModuleDriver>().WriteDirectionData(om, direction);
        }
        #endregion
    }
}
