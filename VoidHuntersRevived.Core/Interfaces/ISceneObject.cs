using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface ISceneObject : IGameObject, IDisposable
    {
        IScene Scene { get; set; }
    }
}
