using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class LayerObject : SceneObject, ILayerObject
    {
        private ILayer _layer;
        public ILayer Layer
        {
            get
            {
                return _layer;
            }
            set
            {
                if (_layer != value)
                {
                    if (_layer != null)
                    { // Only invoke removed from layer if the object has a layer defined value
                        _layer = null;
                        this.OnRemovedFromLayer?.Invoke(this, this);
                    }

                    _layer = value;
                    this.OnAddedToLayer?.Invoke(this, this);
                }
            }
        }

        public event EventHandler<ILayerObject> OnAddedToLayer;
        public event EventHandler<ILayerObject> OnRemovedFromLayer;

        public LayerObject(IGame game) : base(game)
        {
        }
    }
}
