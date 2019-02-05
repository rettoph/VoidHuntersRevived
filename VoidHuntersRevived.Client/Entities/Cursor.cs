using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Client.Entities
{
    /// <summary>
    /// The client side cursor is an entity that contains a single
    /// farseer body (circle) to act as a sensor. The cursor will
    /// allow a single object to be hovered at any time, and the
    /// hovered object can be selected via the GetTarget() method
    /// within the cursor.
    /// 
    /// Other entities, such as the local UserPlayer tractor beam may
    /// be controlled via the cursor as well
    /// </summary>
    public class Cursor : Entity
    {
        #region Private Fields
        private Body _body;
        private Camera _camera;

        private Vector2 _origin;
        #endregion

        public Cursor(EntityInfo info, IGame game) : base(info, game)
        {
        }

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            var scene = this.Scene as ClientMainGameScene;

            // Save the scenes camera
            _camera = scene.Camera;

            // Create a new body
            _body = BodyFactory.CreateCircle(
                scene.World,
                1f,
                0f,
                Vector2.Zero,
                BodyType.Static);

            // Mark the cursor as a sensor
            _body.IsSensor = true;

            // Ensure the cursor is enabled
            this.SetEnabled(true);
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _body.Position = Vector2.Transform(ConvertUnits.ToSimUnits(Mouse.GetState().Position.ToVector2()), _camera.InverseViewMatrix);
        }
        #endregion
    }
}
