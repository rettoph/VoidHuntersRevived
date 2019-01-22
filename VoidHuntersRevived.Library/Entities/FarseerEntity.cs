using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// An entity that integrates directly with farseer,
    /// and has access to the world object
    /// </summary>
    public abstract class FarseerEntity : Entity
    {
        protected World World { get; private set; }

        public FarseerEntity(EntityInfo info, IGame game) : base(info, game)
        {
            this.OnAddedToScene += this.HandleAddedToScene;
            this.OnRemovedFromScene += this.HandleRemovedFromScene;
        }

        protected virtual void HandleAddedToScene(object sender, ISceneObject e)
        {
            this.World = (this.Scene as MainScene).World;
        }

        protected virtual void HandleRemovedFromScene(object sender, ISceneObject e)
        {
            // throw new NotImplementedException();
        }
    }
}
