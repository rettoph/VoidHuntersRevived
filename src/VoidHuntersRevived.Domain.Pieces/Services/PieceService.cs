using Guppy;
using Guppy.Resources.Providers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal class PieceService : GlobalComponent, IPieceService
    {
        private readonly IResourceProvider _resources;

        private Piece[] _pieces;
        private Dictionary<Type, Piece[]> _byDescriptor;
        private Dictionary<string, Piece> _byKey;

        public PieceService(IResourceProvider resources, IEnumerable<Piece> pieces)
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
            _pieces = _pieces.Concat(_resources.GetAll<Piece>().Select(x => x.Item2)).ToArray();
            _byDescriptor = new Dictionary<Type, Piece[]>();
            _byKey = _pieces.ToDictionary(x => x.Key, x => x);
        }

        public Piece[] All<TDescriptor>() where TDescriptor : PieceDescriptor
        {
            ref Piece[]? pieces = ref CollectionsMarshal.GetValueRefOrAddDefault(_byDescriptor, typeof(TDescriptor), out bool exists);

            if (exists == false)
            {
                pieces = _pieces.Where(x => x.Descriptor.GetType().IsAssignableFrom(typeof(TDescriptor))).ToArray();
            }

            return pieces!;
        }

        public Piece[] All()
        {
            return _pieces;
        }

        public Piece GetByKey(string key)
        {
            return _byKey[key];
        }

        public bool TryGetByKey(string key, [MaybeNullWhen(false)] out Piece piece)
        {
            return _byKey.TryGetValue(key, out piece);
        }
    }
}
