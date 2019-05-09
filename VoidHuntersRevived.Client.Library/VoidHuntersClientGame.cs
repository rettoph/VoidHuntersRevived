using System;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client.Library
{
    public class VoidHuntersClientGame : VoidHuntersGame
    {
        public VoidHuntersClientGame(ILogger logger, IServiceProvider provider) : base(logger, provider)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.SetScene(this.CreateScene<VoidHuntersClientWorldScene>());
        }
    }
}