﻿using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Collections;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface IScene : IGameObject
    {
        SceneEntityCollection Entities { get; }
        LayerCollection Layers { get; }
        IServiceProvider Provider { get; }
    }
}
