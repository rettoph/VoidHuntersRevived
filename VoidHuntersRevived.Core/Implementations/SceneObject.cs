using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class SceneObject : GameObject, ISceneObject
    {
        private IScene _scene;
        public IScene Scene
        {
            get
            {
                return _scene;
            }
            set
            {
                if (_scene != null)
                { // Only invoke removed from scene if the object has a scene defined
                    _scene = null;
                    this.OnRemovedFromScene?.Invoke(this, this);
                }

                _scene = value;
                this.OnAddedToScene?.Invoke(this, this);
            }
        }

        public event EventHandler<ISceneObject> OnAddedToScene;
        public event EventHandler<ISceneObject> OnRemovedFromScene;

        public SceneObject(IGame game) : base(game)
        {
        }

        
    }
}
