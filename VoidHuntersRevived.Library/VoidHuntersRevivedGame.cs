using Game = VoidHuntersRevived.Core.Implementations.Game;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Loaders;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersRevivedGame : Game
    {
        public VoidHuntersRevivedGame(ILogger logger, GraphicsDeviceManager graphics = null, ContentManager content = null, GameWindow window = null, IServiceCollection services = null) : base(logger, graphics, content, window, services)
        {
        }
    }
}
