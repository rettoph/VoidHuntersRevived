using Guppy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server
{
    class VoidHuntersServerGame : VoidHuntersGame
    {
        public VoidHuntersServerGame(ILogger logger, IServiceProvider provider) : base(logger, provider)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.SetScene(this.CreateScene<VoidHuntersServerWorldScene>());
        }
    }
}
