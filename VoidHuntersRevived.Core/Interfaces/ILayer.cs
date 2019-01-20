using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Collections;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface ILayer : ISceneObject
    {
        LayerEntityCollection Entities { get; }
    }
}
