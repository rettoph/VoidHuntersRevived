using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IDebugEngine : IEngine
    {
        string? Group => string.Empty;

        void RenderDebugInfo(GameTime gameTime);
    }
}
