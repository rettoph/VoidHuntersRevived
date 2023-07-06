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

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed partial class EntityService : IEntityService, IEngine, IGetReadyEngine
    {
        private readonly IEngineService _engines;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private IEntitySerializationService _serialization;
        private readonly EntityTypeService _entityTypes;
        private readonly DoubleDictionary<VhId, EGID, IdMap> _ids;
        private readonly Dictionary<VhId, EntityType> _types;
        private readonly Queue<IdMap> _removed;
        private readonly ILogger _logger;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public string name { get; } = nameof(EntityService);

        public EntityService(
            ILogger logger,
            IEngineService engines,
            EnginesRoot enginesRoot,
            EntityTypeService entityTypes)
        {
            _engines = null!;
            _factory = null!;
            _functions = null!;
            _entityTypes = entityTypes;
            _ids = new DoubleDictionary<VhId, EGID, IdMap>();
            _types = new Dictionary<VhId, EntityType>();
            _removed = new Queue<IdMap>();
            _logger = logger;
            _factory = enginesRoot.GenerateEntityFactory();
            _functions = enginesRoot.GenerateEntityFunctions();
            _engines = engines;
            _serialization = null!;
        }

        public void Ready()
        {
            _serialization = _engines.Get<IEntitySerializationService>();
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

        public void Clean()
        {
            while(_removed.TryDequeue(out IdMap removed))
            {
                Console.WriteLine($"Removing Entity Map: {removed.VhId}");
                _ids.Remove(removed.VhId, removed.EGID);
                _types.Remove(removed.VhId);
            }
        }
    }
}
