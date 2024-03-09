using Guppy.Resources;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public abstract class PieceDescriptor : VoidHuntersEntityDescriptor
    {
        public PieceDescriptor(Resource<Color> primaryColor, Resource<Color> secondaryColor, int order) : base(primaryColor, secondaryColor, order)
        {
            this.WithInstanceComponents(new ComponentManager[]
            {
                new ComponentManager<Id<PieceType>, PieceTypeIdComponentSerializer>(),
                new ComponentManager<Plug, PlugComponentSerializer>(in Plug.Default),
                new ComponentManager<Coupling, CouplingComponentSerializer>(),
                new ComponentManager<Node, NodeComponentSerializer>(),
                new ComponentManager<Rigid, RigidComponentSerializer>(),
                new ComponentManager<ColorPalette, ColorPaletteComponentSerializer>()
            });

            this.WithStaticComponents(new IComponentBuilder[]
            {
                new ComponentBuilder<Visible>()
            });

            this.WithPostInitializer(this.PostInitialize);
        }

        private void PostInitialize(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<ColorPalette>(new ColorPalette(Color.Red, Color.Green));
        }
    }
}
