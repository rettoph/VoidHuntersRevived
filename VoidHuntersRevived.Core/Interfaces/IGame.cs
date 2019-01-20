using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Collections;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface IGame
    {
        ILogger Logger { get; }
        IServiceProvider Provider { get; }
        SceneCollection Scenes { get; }
        GraphicsDeviceManager Graphics { get; }

        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }
}
