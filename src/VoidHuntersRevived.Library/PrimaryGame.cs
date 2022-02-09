using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library
{
    public abstract class PrimaryGame : Guppy.Game
    {
        public abstract Peer Peer { get; }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.Scenes.Create<PrimaryScene>();
        }
    }
}
