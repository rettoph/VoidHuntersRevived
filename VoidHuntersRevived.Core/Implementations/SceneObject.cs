using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class SceneObject : GameObject, ISceneObject
    {
        public IScene Scene { get; set; }

        public SceneObject(IGame game) : base(game)
        {
        }

        public void Dispose()
        {
            this.Scene = null;
        }
    }
}
