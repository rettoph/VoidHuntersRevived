using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
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
        public Body Body { get; protected set; }

        public FarseerEntity(EntityInfo info, IGame game) : base(info, game)
        {
        }

        protected override void HandleAddedToScene(object sender, ISceneObject e)
        {
            this.World = (this.Scene as MainScene).World;
            this.Body = BodyFactory.CreateBody(world: this.World, userData: this);
        }

        protected override void HandleRemovedFromScene(object sender, ISceneObject e)
        {
            if (this.Body != null)
            {
                this.World.RemoveBody(this.Body);
                this.Body.Dispose();

                this.World = null;
            }
        }
    }
}
