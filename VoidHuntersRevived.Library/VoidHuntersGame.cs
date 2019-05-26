using Guppy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library
{
    public class VoidHuntersGame : Game
    {
        public VoidHuntersGame(
            IServiceProvider provider,
            ILogger logger) : base(provider, logger)
        {
        }
    }
}
