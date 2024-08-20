using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Engines;

namespace VoidHuntersRevived.Domain.Graphics.Providers
{
    [AutoLoad]
    public class VertexTypeEntityEngineProvider : IEngineProvider
    {
        private readonly IVertexTypeService _vertexTypeService;

        public VertexTypeEntityEngineProvider(IVertexTypeService vertexTypeService)
        {
            _vertexTypeService = vertexTypeService;
        }

        public IEnumerable<IEngine> GetEngines()
        {
            IReadOnlyDictionary<Type, IVertexTypeManagerProvider> vertexTypeManagerProvidersByVertexType = _vertexTypeService.GetAllByVertexType();

            foreach ((Type vertexType, IVertexTypeManagerProvider vertexTypeManagerProvider) in vertexTypeManagerProvidersByVertexType)
            {
                if (vertexType.IsAssignableTo<IEntityComponent>() == false)
                {
                    continue;
                }

                yield return VertexTypeEntityEngineProvider.BuildEngine(vertexType, vertexTypeManagerProvider);
            }
        }

        private static IEngine BuildEngine(Type vertexType, IVertexTypeManagerProvider vertexTypeManagerProvider)
        {
            // Many Types - Single Group
            if (vertexTypeManagerProvider.Primitives.Length > 1 && vertexTypeManagerProvider.Groups.Length == 1)
            {
                return VertexTypeEntityEngineProvider.BuildGenericPrimitiveEngine(typeof(PrimitiveEntity_ManyTypesSingleGroup_Engine<>), vertexType, vertexTypeManagerProvider);
            }

            // Many Types - Many Groups
            if (vertexTypeManagerProvider.Primitives.Length > 1 && vertexTypeManagerProvider.Groups.Length > 1)
            {
                return VertexTypeEntityEngineProvider.BuildGenericPrimitiveEngine(typeof(PrimitiveEntity_ManyTypesManyGroups_Engine<>), vertexType, vertexTypeManagerProvider);
            }

            throw new NotImplementedException();
        }

        private static IEngine BuildGenericPrimitiveEngine(Type engineType, Type vertexType, IVertexTypeManagerProvider vertexTypeManagerProvider)
        {
            Type genericEngineType = engineType.MakeGenericType(vertexType);
            ThrowIf.Type.IsNotAssignableFrom<IEngine>(genericEngineType);

            IEngine genericEngine = (IEngine)(Activator.CreateInstance(genericEngineType, [vertexTypeManagerProvider]) ?? throw new NotImplementedException());

            return genericEngine;
        }
    }
}
