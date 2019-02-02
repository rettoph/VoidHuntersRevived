using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Collections;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class Layer : SceneObject, ILayer
    {
        public LayerEntityCollection Entities { get; protected set; }

        public Layer(IGame game) : base(game)
        {
            this.Entities = new LayerEntityCollection(game.Logger, this);

            this.SetEnabled(true);
        }

        public override void Update(GameTime gameTime)
        {
            // Update dirty entities
            // Note: We do not update the entity collection as this will update all contained entities
            // The scene will update its entity collection so it is not neccessary to do so here
            this.Entities.Clean();
        }
    }
}
