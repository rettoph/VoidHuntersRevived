using Guppy;
using Guppy.Resources.Providers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal class PieceTypeService : GlobalComponent, IPieceTypeService
    {
        private readonly IResourceProvider _resources;

        private PieceType[] _pieces;
        private Dictionary<Type, PieceType[]> _byDescriptor;
        private Dictionary<string, PieceType> _byKey;

        public PieceTypeService(IResourceProvider resources, IEnumerable<PieceType> pieces)
        {
            _resources = resources;
            _pieces = pieces.ToArray();
            _byDescriptor = null!;
            _byKey = null!;
        }

        protected override void Initialize(IGlobalComponent[] components)
        {
            base.Initialize(components);

            _resources.Initialize(components);
            _pieces = _pieces.Concat(_resources.GetAll<PieceType>().Select(x => x.Item2)).ToArray();
            _byDescriptor = new Dictionary<Type, PieceType[]>();
            _byKey = _pieces.ToDictionary(x => x.Key, x => x);
        }

        public PieceType[] All<TDescriptor>() where TDescriptor : PieceDescriptor
        {
            ref PieceType[]? pieces = ref CollectionsMarshal.GetValueRefOrAddDefault(_byDescriptor, typeof(TDescriptor), out bool exists);

            if (exists == false)
            {
                pieces = _pieces.Where(x => x.Descriptor.GetType().IsAssignableFrom(typeof(TDescriptor))).ToArray();
            }

            return pieces!;
        }

        public PieceType[] All()
        {
            return _pieces;
        }

        public PieceType GetByKey(string key)
        {
            return _byKey[key];
        }

        public bool TryGetByKey(string key, [MaybeNullWhen(false)] out PieceType piece)
        {
            return _byKey.TryGetValue(key, out piece);
        }
    }
}
