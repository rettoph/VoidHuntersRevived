﻿using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Peers;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Drivers.Entities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using System.Collections.Generic;
using Guppy.Extensions.Collections;
using static VoidHuntersRevived.Client.Library.Services.ButtonService;
using VoidHuntersRevived.Client.Library.Services;
using System.IO;
using Guppy.IO.Services;
using Guppy.IO.Input;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Players
{
    /// <summary>
    /// Simple driver that will manage local controls for 
    /// players owned by the current client.
    /// </summary>
    internal sealed class UserPlayerLocalControllerDriver : NetworkEntityNetworkDriver<UserPlayer>
    {
        #region Private Fields
        private Peer _peer;
        private FarseerCamera2D _camera;
        private Sensor _sensor;
        private ActionTimer _targetSender;
        private MouseService _mouse;
        private ButtonService _keys;
        private Dictionary<Keys, Ship.Direction> _controls;
        private DebugService _debug;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(Object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            _targetSender = new ActionTimer(50);

            provider.Service(out _peer);
            provider.Service(out _camera);
            provider.Service(out _sensor);
            provider.Service(out _mouse);
            provider.Service(out _keys);
            provider.Service(out _debug);

            this.driven.OnUserChanged += this.HandleUserChanged;
        }

        protected override void ConfigureLocal(ServiceProvider provider)
        {
            base.ConfigureLocal(provider);

            this.driven.OnUpdate += this.Update;
            _mouse.OnButtonState[ButtonState.Pressed]  += this.HandleCursorPressed;
            _mouse.OnButtonState[ButtonState.Released] += this.HandleCursorReleased;

            _controls = new Dictionary<Keys, Ship.Direction>();
            _controls.Add(Keys.W, Ship.Direction.Forward);
            _controls.Add(Keys.D, Ship.Direction.TurnRight);
            _controls.Add(Keys.S, Ship.Direction.Backward);
            _controls.Add(Keys.A, Ship.Direction.TurnLeft);
            _controls.Add(Keys.Q, Ship.Direction.Left);
            _controls.Add(Keys.E, Ship.Direction.Right);

            _controls.ForEach(map =>
            {
                _keys[map.Key].OnKeyPressed += this.HandleKeyStateChanged;
                _keys[map.Key].OnKeyReleased += this.HandleKeyStateChanged;
            });

            _keys[Keys.F3].OnKeyPressed += this.SaveShipToFile;

            _debug.Lines += this.RenderDebug;
        }

        protected override void Dispose()
        {
            this.driven.OnUserChanged -= this.HandleUserChanged;
        }

        protected override void DisposeLocal()
        {
            base.DisposeLocal();

            this.driven.OnUpdate -= this.Update;
            _mouse.OnButtonState[ButtonState.Pressed]  -= this.HandleCursorPressed;
            _mouse.OnButtonState[ButtonState.Released] -= this.HandleCursorReleased;

            _controls.ForEach(map =>
            {
                _keys[map.Key].OnKeyPressed -= this.HandleKeyStateChanged;
                _keys[map.Key].OnKeyReleased -= this.HandleKeyStateChanged;
            });
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if(this.driven.Ship != null)
            {
                this.driven.Ship.WorldTarget = _sensor.Position;
                _targetSender.Update(gameTime, () =>
                { // Attempt to send the newest target value...
                    this.WriteUpdateShipTargetRequest(this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 8));
                });

                // Update camera position...
                _camera.MoveTo(this.driven.Ship.Bridge.WorldCenter);
            }
        }

        private String RenderDebug(GameTime gameTime)
            => $"X: {this.driven.Ship.Bridge?.Position.X.ToString("#,##0.00")}, Y: {this.driven.Ship.Bridge?.Position.Y.ToString("#,##0.00")}";
        #endregion

        #region Helper Methods
        private void TrySetShipDirection(Ship.Direction direction, Boolean value)
        {
            if(this.driven.Ship.TrySetDirection(direction, value))
            { // If successful, broadcast a message request...
                this.WriteUpdateShipDirectionRequest(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 7), direction, value);
            }
        }

        private void TrySelectShipTractorBeam()
        {
            var target = _sensor.Contacts
                .Where(c => c is ShipPart)
                .Select(c => (c as ShipPart).Controller is ChunkManager ? (c as ShipPart).Root : (c as ShipPart))
                .Where(s => this.driven.Ship.TractorBeam.CanSelect(s))
                .OrderBy(s => Vector2.Distance(this.driven.Ship.WorldTarget, s.WorldCenter))
                .FirstOrDefault();

            this.HandleTractorBeamAction(this.driven.Ship.TractorBeam.TrySelect(target));
        }

        private void TryDeselectShipTractorBeam()
            => this.HandleTractorBeamAction(this.driven.Ship.TractorBeam.TryDeselect(attach: true));

        /// <summary>
        /// Broadcast a tractorbeam action to the server in the form of an
        /// action request...
        /// </summary>
        /// <param name="action"></param>
        private void HandleTractorBeamAction(TractorBeam.Action action)
        {
            if(action.Type != TractorBeam.ActionType.None)
            { // Only send any request if its not none...
                this.WriteShipTractorBeamActionRequest(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 10), action);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleUserChanged(UserPlayer sender, User arg)
        {
            if(arg == _peer.CurrentUser)
            {
                // Give this specific user local authority...
                this.driven.Authorization |= GameAuthorization.Local;
            }
        }

        private void HandleCursorPressed(InputManager sender, InputArgs args)
            => this.TrySelectShipTractorBeam();

        private void HandleCursorReleased(InputManager sender, InputArgs args)
            => this.TryDeselectShipTractorBeam();

        /// <summary>
        /// Manage a mapped control key being pressed.
        /// </summary>
        /// <param name="sender"></param>
        private void HandleKeyStateChanged(ButtonManager sender, ButtonService.ButtonValue args)
            => this.TrySetShipDirection(_controls[args.Which.KeyboardKey], args.State == ButtonState.Pressed);

        /// <summary>
        /// Save the current ship to a file.
        /// </summary>
        /// <param name="sender"></param>
        private void SaveShipToFile(ButtonManager sender, ButtonService.ButtonValue args)
        {
            Directory.CreateDirectory("ships");
            using (FileStream file = File.Open("ships/test.vh", FileMode.Create))
            {
                using(MemoryStream ship = this.driven.Ship.Export())
                {
                    var data = ship.ToArray();
                    file.Write(data, 0, data.Length);
                    file.Flush();
                }
            }
        }
        #endregion

        #region Network Methods
        private void WriteUpdateShipDirectionRequest(NetOutgoingMessage om, Ship.Direction direction, Boolean value)
            => om.Write("update:ship:direction:request", m =>
            {
                m.Write((Byte)direction);
                m.Write(value);
            });

        private void WriteUpdateShipTargetRequest(NetOutgoingMessage om)
            => om.Write("update:ship:target:request", m =>
            {
                m.Write(this.driven.Ship.Target);
            });

        private void WriteShipTractorBeamActionRequest(NetOutgoingMessage om, TractorBeam.Action action)
        {
            om.Write("ship:tractor-beam:action:request", m =>
            {
                om.Write((Byte)action.Type);
                om.Write(action.Target);
            });
        }
        #endregion
    }
}
