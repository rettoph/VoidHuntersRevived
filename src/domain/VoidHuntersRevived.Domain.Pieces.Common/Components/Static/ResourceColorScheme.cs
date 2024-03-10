using Guppy.Resources;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Common.Pieces.Components.Shared
{
    public struct ResourceColorScheme : IEntityComponent, IPieceComponent
    {
        public struct ResourceColorSchemeValue
        {
            private readonly Guid _resourceId;

            public Resource<Color> Resource => Guppy.Resources.Resource.Get<Color>(_resourceId);

            public Color Value;

            public ResourceColorSchemeValue(Resource<Color> resource)
            {
                _resourceId = resource.Id;
            }
        }

        public ResourceColorSchemeValue Primary;
        public ResourceColorSchemeValue Secondary;

        public ResourceColorScheme(Resource<Color> primary, Resource<Color> secondary)
        {
            Primary = new ResourceColorSchemeValue(primary);
            Secondary = new ResourceColorSchemeValue(secondary);
        }
    }
}
