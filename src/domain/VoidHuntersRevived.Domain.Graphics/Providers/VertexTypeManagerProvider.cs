using Guppy.Core.Common.Extensions.System;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;
using VoidHuntersRevived.Domain.Graphics.Utilities;

namespace VoidHuntersRevived.Domain.Graphics.Providers
{
    public class VertexTypeManagerProvider<TVertex> : IVertexTypeManagerProvider<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        private static readonly FilterContextID VertexTypeManagerFilterContextId = FilterContextID.GetNewContextID();

        private readonly Dictionary<PrimitiveGroupEnum, IVertexTypeManager<TVertex>> _vertexTypeManagersByGroup;
        private readonly IVertexTypeManager<TVertex>[] _orderedVertexTypeManagers;
        public Type VertexType => typeof(TVertex);

        public IPrimitive<TVertex>[] Primitives { get; }
        public PrimitiveGroupEnum[] Groups { get; }

        IPrimitive[] IVertexTypeManagerProvider.Primitives => this.Primitives;

        public VertexTypeManagerProvider(
            IUniqueNumberProvider uniqueNumberProvider,
            IPrimitive[] primitives)
        {
            if (primitives.Any(x => x is not IPrimitive<TVertex>))
            {
                throw new ArgumentException($"Not all primitives are of type {typeof(IPrimitive<TVertex>).GetFormattedName()}", nameof(primitives));
            }

            this.Primitives = primitives.OfType<IPrimitive<TVertex>>().ToArray();
            this.Groups = this.Primitives.SelectMany(x => x.Groups).Distinct().Order().ToArray();

            _vertexTypeManagersByGroup = this.Groups.ToDictionary(
                keySelector: g => g,
                elementSelector: g => (IVertexTypeManager<TVertex>)new VertexTypeManager<TVertex>(
                    group: g,
                    filterId: new CombinedFilterID(uniqueNumberProvider.GetInt32(), VertexTypeManagerFilterContextId),
                    primitives: this.Primitives.Where(p => p.Groups.Contains(g)).ToArray())
                );

            _orderedVertexTypeManagers = _vertexTypeManagersByGroup.Values.OrderBy(x => x.Group).ToArray();
        }

        public IVertexTypeManager<TVertex> GetByGroup(PrimitiveGroupEnum group)
        {
            return _vertexTypeManagersByGroup[group];
        }

        public IEnumerable<IVertexTypeManager<TVertex>> GetAll()
        {
            return _orderedVertexTypeManagers;
        }

        IVertexTypeManager IVertexTypeManagerProvider.GetByGroup(PrimitiveGroupEnum group)
        {
            return this.GetByGroup(group);
        }

        IEnumerable<IVertexTypeManager> IVertexTypeManagerProvider.GetAll()
        {
            return this.GetAll();
        }
    }
}
