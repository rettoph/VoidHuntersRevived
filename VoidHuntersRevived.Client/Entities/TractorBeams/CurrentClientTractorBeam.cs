using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.TractorBeams;

namespace VoidHuntersRevived.Client.Entities.TractorBeams
{
    /// <summary>
    /// A specific version of the tractor beam entity 
    /// controllable by the current client
    /// </summary>
    public class CurrentClientTractorBeam : TractorBeam
    {
        private MainSceneClient _scene;

        public CurrentClientTractorBeam(EntityInfo info, IGame game) : base(info, game)
        {
        }

        protected override void HandleAddedToScene(object sender, ISceneObject e)
        {
            base.HandleAddedToScene(sender, e);

            _scene = this.Scene as MainSceneClient;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Body.Position = Vector2.Transform(
                ConvertUnits.ToSimUnits(Mouse.GetState().Position.ToVector2()),
                _scene.Camera.InverseViewMatrix);
        }
    }
}
