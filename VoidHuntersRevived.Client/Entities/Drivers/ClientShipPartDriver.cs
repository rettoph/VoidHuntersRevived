using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Entities.Drivers
{
    /// <summary>
    /// The client specific implementation of the IDriver, designed
    /// specifically for ShipPart instances.
    /// </summary>
    public class ClientShipPartDriver : Driver
    {
        #region Private Fields
        // The parent ShipPart
        private ShipPart _parent;

        // The parent ShipPart's position, according to the servers driver
        private Vector2 _position;
        // The parent ShipPart's linear velocity, according to the servers driver
        private Vector2 _linearVelocity;

        // The parent ShipPart's rotation, according to the servers driver
        private Single _rotation;
        // The parent ShipPart's angular velocity, according to the servers driver
        private Single _angularVelocity;

        // The lerp strength (multiplied by the number om milliseconds per frame)
        private Single _baseLerpStrength;
        private Single _currentLerpStrength;

        // If the server gives a value further away than these threshold values, the ship part will snap to its location
        private Single _positionSnapThreshold;
        private Single _rotationSnapThreshold;
        #endregion

        #region Constructors
        public ClientShipPartDriver(ShipPart parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;

            // Set the base positional values to the parents current positional values
            _position       = _parent.Body.Position;
            _linearVelocity = _parent.Body.LinearVelocity;
             
            _rotation        = _parent.Body.Rotation;
            _angularVelocity = _parent.Body.AngularVelocity;

            // Set the default lerp strength
            _baseLerpStrength = 0.01f;

            // Set default snip thresholds
            _positionSnapThreshold = 1;
            _rotationSnapThreshold = (float)Math.PI / 4;
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Calculate the lerp strength from the elapsed game time
            _currentLerpStrength = _baseLerpStrength * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Update the parent ShipPart's positional values based on what was recieved from the server
            _parent.Body.Position       = Vector2.Distance(_parent.Body.Position, _position) > _positionSnapThreshold ? _position : Vector2.Lerp(_parent.Body.Position, _position, _currentLerpStrength);
            _parent.Body.LinearVelocity = Vector2.Distance(_parent.Body.Position, _linearVelocity) > _positionSnapThreshold ? _linearVelocity : Vector2.Lerp(_parent.Body.LinearVelocity, _linearVelocity, _currentLerpStrength);

            _parent.Body.Rotation        = Math.Abs(_parent.Body.Rotation - _rotation) > _rotationSnapThreshold ? _rotation : MathHelper.Lerp(_parent.Body.Rotation, _rotation, _currentLerpStrength);
            _parent.Body.AngularVelocity = Math.Abs(_parent.Body.Rotation - _angularVelocity) > _rotationSnapThreshold ? _angularVelocity :  MathHelper.Lerp(_parent.Body.AngularVelocity, _angularVelocity, _currentLerpStrength);
        }
        #endregion

        #region Networking Methods (Driver Implementation)
        public override void Read(NetIncomingMessage im)
        {
            // Read incoming positional data sent from the servers driver
            _position       = im.ReadVector2();
            _linearVelocity = im.ReadVector2();

            _rotation        = im.ReadSingle();
            _angularVelocity = im.ReadSingle();
        }

        public override void Write(NetOutgoingMessage om)
        { // The client should never write ship part data to the server
            throw new NotImplementedException();
        }
        #endregion
    }
}
