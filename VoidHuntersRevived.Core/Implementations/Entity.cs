﻿using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class Entity : LayerObject, IEntity
    {
        public EntityInfo Info { get; protected set; }

        public Entity(EntityInfo info, IGame game) : base(game)
        {
            this.Info = info;

            this.Visible = true;
            this.Enabled = true;

            this.OnAddedToScene += this.HandleAddedToScene;
            this.OnRemovedFromScene += this.HandleRemovedFromScene;
        }

        protected virtual void HandleRemovedFromScene(object sender, ISceneObject e)
        {
            // throw new NotImplementedException();
        }

        protected virtual void HandleAddedToScene(object sender, ISceneObject e)
        {
            // throw new NotImplementedException();
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }
    }
}
