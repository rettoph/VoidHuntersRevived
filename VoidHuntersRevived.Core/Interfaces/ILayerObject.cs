using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface ILayerObject : ISceneObject
    {
        ILayer Layer { get; set; }
        event EventHandler<ILayerObject> OnAddedToLayer;
        event EventHandler<ILayerObject> OnRemovedFromLayer;
    }
}
