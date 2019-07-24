using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Guppy.Implementations;
using Guppy.Network.Peers;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    /// <summary>
    /// The client user player driver is specifically for interfacing
    /// directly with external inputs and doing actions to the players
    /// ship.
    /// </summary>
    public class ClientUserPlayerDriver : Driver
    {
        #region Private Fields
        private FarseerCamera2D _camera;
        private Pointer _pointer;
        private ClientPeer _client;
        private UserPlayer _player;
        private World _world;
        private Body _sensor;
        private HashSet<ShipPart> _contacts;
        #endregion

        #region Constructors
        public ClientUserPlayerDriver(World world, FarseerCamera2D camera, Pointer pointer, ClientPeer client, UserPlayer parent, IServiceProvider provider) : base(parent, provider)
        {
            _world = world;
            _camera = camera;
            _pointer = pointer;
            _client = client;
            _player = parent;
            _contacts = new HashSet<ShipPart>();
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            if (_player.User == _client.CurrentUser)
            {
                _sensor = BodyFactory.CreateCircle(_world, 1, 0f, Vector2.Zero, BodyType.Dynamic, null);
                _sensor.IsSensor = true;
                _sensor.SleepingAllowed = false;
                _sensor.CollisionCategories = Category.Cat1;
                _sensor.CollidesWith = Category.All;

                _sensor.OnCollision += this.HandleSensorCollision;
                _sensor.OnSeparation += this.HandleSensorSeperation;
                _pointer.OnLocalMovementEnded += this.HandlePointerLocalMovementEnded;
            }
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (_client.CurrentUser == _player.User && _player.Ship != null)
            { // If the current player represents the locally signed in user...
                var kState = Keyboard.GetState();

                if(_player.Ship.Bridge != null)
                { // Bridge specific actions...

                    Boolean d;
                    if ((d = kState.IsKeyDown(Keys.W)) != _player.Ship.GetDirection(Direction.Forward))
                        this.SetLocalDirection(Direction.Forward, d);
                    if ((d = kState.IsKeyDown(Keys.A)) != _player.Ship.GetDirection(Direction.TurnLeft))
                        this.SetLocalDirection(Direction.TurnLeft, d);
                    if ((d = kState.IsKeyDown(Keys.S)) != _player.Ship.GetDirection(Direction.Backward))
                        this.SetLocalDirection(Direction.Backward, d);
                    if ((d = kState.IsKeyDown(Keys.D)) != _player.Ship.GetDirection(Direction.TurnRight))
                        this.SetLocalDirection(Direction.TurnRight, d);

                    // Update the camera position
                    _camera.MoveTo(_player.Ship.Bridge.Position);

                    // Update the tractor beam target sensor
                    _sensor.SetTransform(_pointer.Position, 0);
                    _player.Ship.TractorBeam.SetOffset(_pointer.Position - _player.Ship.Bridge.Position);

                    if(_player.Ship.TractorBeam.Selecting)
                    {
                        if (!_pointer.Secondary)
                            this.TryTractorBeamLocalRelease();
                    }
                    else
                    {
                        var hovered = _player.Ship.TractorBeam.GetTarget(
                            _contacts.Where(sp => _player.Ship.TractorBeam.ValidateTarget(sp))
                                .OrderBy(sp => Vector2.Distance(_player.Ship.TractorBeam.Position, sp.Position))
                                .FirstOrDefault());

                        if (_pointer.Secondary)
                            this.TryTractorBeamLocalSelect(hovered);
                    }
                }

                // Update the camera zoom as needed...
                if(_pointer.ScrollDelta != 0)
                    _camera.ZoomBy(1 + 0.1f * ((Single)_pointer.ScrollDelta / 120));
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Update the local direction to a specified value,
        /// but also send a message to the server alerting it
        /// of this change.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        private void SetLocalDirection(Direction direction, Boolean value)
        {
            _player.Ship.SetDirection(direction, value);

            var action = _player.CreateActionMessage("set:direction");
            _player.Ship.WriteDirectionData(action, direction);
        }

        private void TryTractorBeamLocalSelect(ShipPart target)
        {
            if(_player.Ship.TractorBeam.TrySelect(target))
            { // If the local select was successfull...
                var action = _player.CreateActionMessage("tractor-beam:select");
                _player.Ship.TractorBeam.WriteOffsetData(action);
                _player.Ship.TractorBeam.WriteSelectedData(action);
            }
        }

        private void TryTractorBeamLocalRelease()
        {
            if(_player.Ship.TractorBeam.TryRelease())
            { // If the local release was successfull...
                var action = _player.CreateActionMessage("tractor-beam:release");
                _player.Ship.TractorBeam.WriteOffsetData(action);
            }

        }
        #endregion

        #region Event Handlers
        private bool HandleSensorCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureA.UserData is ShipPart)
                _contacts.Add(fixtureA.UserData as ShipPart);
            else if (fixtureB.UserData is ShipPart)
                _contacts.Add(fixtureB.UserData as ShipPart);

            return true;
        }

        private void HandleSensorSeperation(Fixture fixtureA, Fixture fixtureB)
        {
            if (fixtureA.UserData is ShipPart)
                _contacts.Remove(fixtureA.UserData as ShipPart);
            else if (fixtureB.UserData is ShipPart)
                _contacts.Remove(fixtureB.UserData as ShipPart);
        }

        private void HandlePointerLocalMovementEnded(object sender, Vector2 e)
        {
            var action = _player.CreateActionMessage("tractor-beam:set:offset");
            _player.Ship.TractorBeam.WriteOffsetData(action);
        }
        #endregion
    }
}
