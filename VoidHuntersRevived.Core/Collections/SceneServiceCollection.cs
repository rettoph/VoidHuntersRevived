using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Collections
{
    public class SceneServiceCollection : GameObjectCollection<ISceneService>
    {
        private IScene _scene;

        public SceneServiceCollection(ILogger logger, IScene scene) : base(logger)
        {
            _scene = scene;
        }

        protected override bool add(ISceneService item)
        {
            if (base.add(item))
            {
                item.Scene = _scene;

                return true;
            }

            return false;
        }

        protected override bool remove(ISceneService item)
        {
            if (base.remove(item))
            {
                item.Scene = null;

                return true;
            }

            return false;
        }
    }
}
