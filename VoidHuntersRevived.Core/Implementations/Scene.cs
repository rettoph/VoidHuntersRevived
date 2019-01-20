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

        public Scene(IGame game) : base(game)
        {
            this.Entities = new SceneEntityCollection(this.Game.Logger, this);
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            this.Entities.Update(gameTime);
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
