using Guppy.Common.Collections;
using Serilog;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using Svelto.ECS.Serialization;
using System.Text;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Domain.Entities.Abstractions;
using VoidHuntersRevived.Domain.Entities.EnginesGroups;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed partial class EntityService : IEntityService, IEngine, IEnginesGroupEngine
    {
        private IEngineService _engines;
        private IEntityFactory _factory;
        private IEntityFunctions _functions;
        private readonly EntityTypeService _entityTypes;
        private readonly DoubleDictionary<VhId, EGID, IdMap> _ids;
        private readonly Dictionary<VhId, EntityType> _types;
        private readonly ILogger _logger;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityService(EntityTypeService entityTypes, ILogger logger)
        {
            _engines = null!;
            _factory = null!;
            _functions = null!;
            _entityTypes = entityTypes;
            _ids = new DoubleDictionary<VhId, EGID, IdMap>();
            _types = new Dictionary<VhId, EntityType>();
            _logger = logger;
        }

        public void Initialize(IEngineService engines)
        {
            _engines = engines;
            _factory = engines.Root.GenerateEntityFactory();
            _functions = engines.Root.GenerateEntityFunctions();
        }

        public IdMap GetIdMap(VhId vhid)
        {
            return _ids[vhid];
        }

        public IdMap GetIdMap(EGID egid)
        {
            return _ids[egid];
        }

        public IdMap GetIdMap(uint entityId, ExclusiveGroupStruct groupId)
        {
            return this.GetIdMap(new EGID(entityId, groupId));
        }

        public bool TryGetIdMap(VhId vhid, out IdMap id)
        {
            return _ids.TryGet(vhid, out id);
        }

        public bool TryGetIdMap(EGID egid, out IdMap id)
        {
            return _ids.TryGet(egid, out id);
        }

        public bool TryGetIdMap(uint entityId, ExclusiveGroupStruct groupId, out IdMap id)
        {
            return _ids.TryGet(new EGID(entityId, groupId), out id);
        }

        public EntityType GetEntityType(VhId entityVhId)
        {
            return _types[entityVhId];
        }
    }
}
