﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IDrawDebuggerEngine
    {
        string Group => string.Empty;

        void DrawDebugger(GameTime gameTime);
    }
}
