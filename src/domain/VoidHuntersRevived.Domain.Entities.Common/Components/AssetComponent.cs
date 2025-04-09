using Guppy.Core.Assets.Common;
using Guppy.Core.Assets.Common.Services;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public readonly struct AssetComponent<T>(Asset<T> value) : IEntityComponent
        where T : notnull
    {
        private readonly Asset<T> _value = value;

        public AssetKey<T> Asset => this._value.Key;
        public T Value => this._value.Value;

        public AssetComponent(string name, IAssetService resourceService) : this(resourceService.Get(AssetKey<T>.Get(name)))
        {
        }
    }
}