using Guppy.GUI;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Client.GameComponents
{
    internal sealed class ScreenComponent : SimpleGameComponent
    {
        private readonly IScreen _screen;

        public ScreenComponent(IScreen screen)
        {
            _screen = screen;
        }

        public override void Update(GameTime gameTime)
        {
            _screen.Camera.Update(gameTime);
        }
    }
}
