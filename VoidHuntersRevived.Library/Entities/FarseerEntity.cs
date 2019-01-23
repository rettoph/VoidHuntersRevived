using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Scenes.Interfaces;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// An entity that integrates directly with farseer,
    /// and has access to the world object
    /// </summary>
    public abstract class FarseerEntity : Entity, IFarseerEntity
    {
        public World World { get; private set; }
        public Body Body { get; protected set; }

        public FarseerEntity(EntityInfo info, IGame game) : base(info, game)
        {
        }

        protected override void Initialize()
        {
            this.World = (this.Scene as IFarseerScene).World;
            this.Body = BodyFactory.CreateBody(world: this.World, userData: this);
        }
    }
}
