using GalacticFighters.Client.Library.Entities;
using GalacticFighters.Client.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers
{
    [IsDriver(typeof(Pointer))]
    public class MousePointerDriver : Driver<Pointer>
    {
        private ClientGalacticFightersWorldScene _scene;
        private Vector2 _mouse;

        public MousePointerDriver(Pointer driven, ClientGalacticFightersWorldScene scene) : base(driven)
        {
            _scene = scene;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mState = Mouse.GetState();

            // Move the pointer to the recieved mouse position
            this.driven.MoveTo(mState.Position.ToVector2());
        }
    }
}
