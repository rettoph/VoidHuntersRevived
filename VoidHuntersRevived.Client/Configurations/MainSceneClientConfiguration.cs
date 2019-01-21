using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Services;
using VoidHuntersRevived.Core.Configurations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Client.Configurations
{
    class MainSceneClientConfiguration : ISceneConfiguration
    {
        public void Configure(IScene scene)
        {
            scene.Services.Create<FarseerDebugOverlayService>();
        }
    }
}
