using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface ISceneObject : IGameObject
    {
        IScene Scene { get; set; }
        event EventHandler<ISceneObject> OnAddedToScene;
        event EventHandler<ISceneObject> OnRemovedFromScene;
    }
}
