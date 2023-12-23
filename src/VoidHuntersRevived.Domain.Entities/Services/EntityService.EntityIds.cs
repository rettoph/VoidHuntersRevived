using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        private readonly Dictionary<VhId, EntityId> _ids = new Dictionary<VhId, EntityId>();
        private readonly Queue<EntityModificationRequest> _modifications = new Queue<EntityModificationRequest>();

        public string name { get; } = nameof(EntityService);

        public EntityId GetId(VhId vhid)
        {
            try
            {
                return _ids[vhid];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool TryGetId(VhId vhid, out EntityId id)
        {
            return _ids.TryGetValue(vhid, out id);
        }

        private bool AddId(EntityId id)
        {
            if (_ids.TryAdd(id.VhId, id))
            {
                return true;
            }

            throw new Exception();
        }

        private void EnqueuEntityModification(EntityModificationRequest request)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityModificationType = {EntityModificationType}, EntityId = {EntityId}", nameof(EntityService), nameof(EnqueuEntityModification), request.ModificationType, request.Id.VhId);
            _modifications.Enqueue(request);
        }

        private bool RemoveId(EntityId id)
        {
            if (_ids.Remove(id.VhId))
            {
                return true;
            }

            throw new Exception();
        }
    }
}
