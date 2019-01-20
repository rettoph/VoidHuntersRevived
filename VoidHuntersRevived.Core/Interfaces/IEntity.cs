using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface IEntity : ISceneObject
    {
        EntityInfo Info { get; }
    }
}
