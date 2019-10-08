using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GalacticFighters.Client.Library.Entities;
using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.Players;
using Guppy;
using Guppy.Attributes;
using Guppy.Network.Peers;
using Guppy.UI.Entities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Entities.ShipParts.ConnectionNodes;
using Microsoft.Xna.Framework.Graphics;
using Guppy.Loaders;
using Microsoft.Extensions.Logging;
using GalacticFighters.Library.Utilities;
using System.IO;

namespace GalacticFighters.Client.Library.Drivers.Entities.Players
{
    [IsDriver(typeof(UserPlayer))]
    public class ClientUserPlayerDriver : Driver<UserPlayer>
    {
        #region Static Attributes
        public static Double UpdateTargetRate { get; set; } = 50;
        #endregion

        #region Private Fields
        private Pointer _pointer;
        private World _world;
        private ClientPeer _client;
        private ClientGalacticFightersWorldScene _scene;
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;
        private Double _lastUpdateTarget;
        private ShipBuilder _builder;
        private DebugOverlay _debugOverlay;
        #endregion

        #region Constructor
        public ClientUserPlayerDriver(DebugOverlay debugOverlay, ShipBuilder builder, ContentLoader content, SpriteBatch spriteBatch, Pointer pointer, World world, ClientPeer client, ClientGalacticFightersWorldScene scene, UserPlayer driven) : base(driven)
        {
            _debugOverlay = debugOverlay;
            _builder = builder;
            _font = content.TryGet<SpriteFont>("font");
            _spriteBatch = spriteBatch;
            _pointer = pointer;
            _world = world;
            _client = client;
            _scene = scene;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            if(this.driven.User == _client.User)
            {
                _pointer.Events.TryAdd<Pointer.Button>("pressed", this.HandlePointerButtonPressed);
                _pointer.Events.TryAdd<Pointer.Button>("released", this.HandlePointerButtonReleased);
                _pointer.Events.TryAdd<Int32>("scrolled", this.HandlePointerScrolled);

                _debugOverlay.AddLine(() => $"Position: {this.driven.Ship?.Bridge?.Position.X.ToString("#0.##0")}, {this.driven.Ship?.Bridge?.Position.Y.ToString("#0.##0")}");
                _debugOverlay.AddLine(() => $"Velocity: {this.driven.Ship?.Bridge?.LinearVelocity.Length().ToString("#0.##0")}");
                _debugOverlay.AddLine(() => $"Size: {this.driven.Ship?.Size}");
                _debugOverlay.AddLine(() => $"Open Nodes: {this.driven.Ship?.OpenFemaleNodes.Count()}");
            }
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.driven.Ship != null && this.driven.Ship.Bridge != null && this.driven.User == _client.User)
            { // If the current UserPlayer instance is owned by the local user...
                var kState = Keyboard.GetState();

                this.UpdateDirection(Ship.Direction.Forward, kState.IsKeyDown(Keys.W));
                this.UpdateDirection(Ship.Direction.TurnRight, kState.IsKeyDown(Keys.D));
                this.UpdateDirection(Ship.Direction.Backward, kState.IsKeyDown(Keys.S));
                this.UpdateDirection(Ship.Direction.TurnLeft, kState.IsKeyDown(Keys.A));

                this.UpdateDirection(Ship.Direction.Left, kState.IsKeyDown(Keys.Q));
                this.UpdateDirection(Ship.Direction.Right, kState.IsKeyDown(Keys.E));

                // Update the camera position
                _scene.Camera.MoveTo(this.driven.Ship.Bridge.WorldCenter);

                // Update the Ship's target offset
                this.driven.Ship.SetTargetOffset(_scene.Sensor.WorldCenter - this.driven.Ship.Bridge.WorldCenter);

                _lastUpdateTarget += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_lastUpdateTarget >= ClientUserPlayerDriver.UpdateTargetRate)
                { // Send the vitals data to all connected clients
                    var om = this.driven.Actions.Create("target:changed:request");
                    // Write the vitals data
                    om.Write(this.driven.Ship.TargetOffset);

                    _lastUpdateTarget = _lastUpdateTarget % ClientUserPlayerDriver.UpdateTargetRate;
                }
            }
        }
        #endregion

        #region Helper Methods

        #endregion

        #region Input Handlers
        private void UpdateDirection(Ship.Direction direction, Boolean value)
        {
            if(this.driven.Ship.ActiveDirections.HasFlag(direction) != value)
            { // If the flag has not already been updated...
                // Update the local ship, so the local user feels immediate response...
                this.driven.Ship.SetDirection(direction, value);

                // Create an action to relay back to the server
                var action = this.driven.Actions.Create("direction:changed:request");
                action.Write((Byte)direction);
                action.Write(value);
            }
        }
        #endregion

        #region Event Handlers
        private void HandlePointerButtonPressed(object sender, Pointer.Button button)
        {
            // Immediately attempt to select the local tractorbeam
            var target = _scene.Sensor.Contacts
                    .Where(sp => this.driven.Ship.TractorBeam.ValidateTarget(sp))
                    .OrderBy(sp => MathHelper.Min(Vector2.Distance(sp.MaleConnectionNode.WorldPosition, _scene.Sensor.WorldCenter), Vector2.Distance(sp.WorldCenteroid, _scene.Sensor.WorldCenter)))
                    .FirstOrDefault();

            if (this.driven.Ship.TractorBeam.TrySelect(target))
            { // Write an action to the server...
                var action = this.driven.Actions.Create("tractor-beam:selected:request");
                this.driven.Ship.WriteTargetOffset(action);
                action.Write(this.driven.Ship.TractorBeam.Selected.Id);
            }
        }

        private void HandlePointerButtonReleased(object sender, Pointer.Button button)
        {
            var target = this.driven.Ship.GetClosestOpenFemaleNode(this.driven.Ship.Target);

            if(target == default(FemaleConnectionNode))
            { // If there is no valid open female node...
                if (this.driven.Ship.TractorBeam.TryRelease())
                { // Write a release action to the server
                    var action = this.driven.Actions.Create("tractor-beam:released:request");
                    this.driven.Ship.WriteTargetOffset(action);
                }
            }
            else
            { // If there is a valid open female node...
                if (this.driven.Ship.TractorBeam.TryAttach(target))
                { // Write an attach action to the server
                    var action = this.driven.Actions.Create("tractor-beam:attached:request");
                    action.Write(target.Parent);
                    action.Write(target.Id);

                    // Small patch, export the ship in its current state
                    using (FileStream output = File.OpenWrite("ship.vh"))
                    {
                        _builder.Export(this.driven.Ship.Bridge).WriteTo(output);
                    }
                }
            }

        }

        private void HandlePointerScrolled(object sender, Int32 arg)
        { // Zoom in the camera
            _scene.Camera.ZoomTo((Single)Math.Pow(1.5, (Single)arg / 120));
        }
        #endregion
    }
}
