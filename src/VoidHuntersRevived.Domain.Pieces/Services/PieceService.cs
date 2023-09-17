using Guppy.Resources.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal class PieceService : IPieceService
    {
        private Piece[] _pieces;
        private Dictionary<Type, Piece[]> _byDescriptor;

        public PieceService(IResourceProvider resources, IEnumerable<Piece> pieces)
        {
            _pieces = pieces.Concat(resources.GetAll<Piece>().Select(x => x.Item2)).ToArray();
            _byDescriptor = new Dictionary<Type, Piece[]>();
        }

        public Piece[] All<TDescriptor>() where TDescriptor : PieceDescriptor
        {
            ref Piece[]? pieces = ref CollectionsMarshal.GetValueRefOrAddDefault(_byDescriptor, typeof(TDescriptor), out bool exists);

            if(exists == false)
            {
                pieces = _pieces.Where(x => x.Descriptor.GetType().IsAssignableFrom(typeof(TDescriptor))).ToArray();
            }

            return pieces!;
        }

        public Piece[] All()
        {
            return _pieces;
        }
    }
}
