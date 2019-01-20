using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Collections;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Implementations
{
    public class Scene : GameObject, IScene
    {
        public SceneEntityCollection Entities { get; protected set; }
        public LayerCollection Layers { get; protected set; }
        public IServiceProvider Provider { get; protected set; }

        public Scene(IServiceProvider provider, IGame game) : base(game)
        {
            this.Entities = new SceneEntityCollection(this.Game.Logger, this);
            this.Layers = new LayerCollection(this.Game.Logger, this);
            this.Provider = provider;
        }

        public override void Draw(GameTime gameTime)
        {
            this.Layers.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            this.Entities.Update(gameTime);
            this.Layers.Update(gameTime);
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
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
    }
}
