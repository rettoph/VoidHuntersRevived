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

namespace GalacticFighters.Client.Library.Drivers.Entities.Players
{
    [IsDriver(typeof(UserPlayer))]
    public class ClientUserPlayerDriver : Driver<UserPlayer>
    {
        #region Private Fields
        private Pointer _pointer;
        private World _world;
        private ClientPeer _client;
        private ClientGalacticFightersWorldScene _scene;
        #endregion

        #region Constructor
        public ClientUserPlayerDriver(Pointer pointer, World world, ClientPeer client, ClientGalacticFightersWorldScene scene, UserPlayer driven) : base(driven)
        {
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
            }
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.driven.Ship.Bridge != null && this.driven.User == _client.User)
            { // If the current UserPlayer instance is owned by the local user...
                var kState = Keyboard.GetState();

                this.UpdateDirection(Ship.Direction.Forward, kState.IsKeyDown(Keys.W));
                this.UpdateDirection(Ship.Direction.Right, kState.IsKeyDown(Keys.D));
                this.UpdateDirection(Ship.Direction.Backward, kState.IsKeyDown(Keys.S));
                this.UpdateDirection(Ship.Direction.Left, kState.IsKeyDown(Keys.A));

                // Update the camera position
                _scene.Camera.MoveTo(this.driven.Ship.Bridge.WorldCenter);

                // Update the tractor beam position
                this.driven.Ship.TractorBeam.SetOffset(_scene.Sensor.WorldCenter - this.driven.Ship.Bridge.WorldCenter);

                if(this.driven.Ship.TractorBeam.Selected != default(ShipPart))
                { // Ghost snap the selected ship part, giving the user a placement preview
                    var node = this.driven.Ship.GetClosestOpenFemaleNode(this.driven.Ship.TractorBeam.Position);

                    if(node != default(FemaleConnectionNode))
                    { // Only proceed if there is a valid female node...
                        // Rather than creating the attachment, we just want to move the selection
                        // so that a user can preview what it would look like when attached.
                        var previewRotation = node.WorldRotation - this.driven.Ship.TractorBeam.Selected.MaleConnectionNode.LocalRotation;
                        // Update the preview position
                        this.driven.Ship.TractorBeam.Selected.SetPosition(
                            position: node.WorldPosition - Vector2.Transform(this.driven.Ship.TractorBeam.Selected.MaleConnectionNode.LocalPosition, Matrix.CreateRotationZ(previewRotation)),
                            rotation: previewRotation);
                    }
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
                    .OrderBy(sp => Vector2.Distance(sp.MaleConnectionNode.WorldPosition, _scene.Sensor.WorldCenter))
                    .FirstOrDefault();

            if (this.driven.Ship.TractorBeam.TrySelect(target))
            { // Write an action to the server...
                var action = this.driven.Actions.Create("tractor-beam:selected:request");
                action.Write(this.driven.Ship.TractorBeam.Offset);
                action.Write(this.driven.Ship.TractorBeam.Selected.Id);
            }
        }

        private void HandlePointerButtonReleased(object sender, Pointer.Button button)
        {
            var target = this.driven.Ship.GetClosestOpenFemaleNode(this.driven.Ship.TractorBeam.Position);

            if(target == default(FemaleConnectionNode))
            { // If there is no valid open female node...
                if (this.driven.Ship.TractorBeam.TryRelease())
                { // Write a release action to the server
                    var action = this.driven.Actions.Create("tractor-beam:released:request");
                    action.Write(this.driven.Ship.TractorBeam.Offset);
                }
            }
            else
            { // If there is a valid open female node...
                if (this.driven.Ship.TractorBeam.TryAttach(target))
                { // Write an attach action to the server
                    var action = this.driven.Actions.Create("tractor-beam:attached:request");
                    action.Write(target.Parent);
                    action.Write(target.Id);
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
