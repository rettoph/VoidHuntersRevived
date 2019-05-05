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
            ILogger logger, 
            IServiceProvider provider) : base(logger, provider)
        {
        }
    }
}
