using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorBeam : FarseerEntity
    {
        public Vector2 Offset { get; private set; }

        public event EventHandler<Vector2> OnOffsetChanged;

        public TractorBeam(EntityConfiguration configuration, VoidHuntersWorldScene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
        }
        public TractorBeam(Guid id, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, scene, provider, logger)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void SetOffset(Vector2 offset)
        {
            if(this.Offset != offset)
            {
                this.Offset = offset;

                this.OnOffsetChanged?.Invoke(this, this.Offset);
            }
        }
    }
}
