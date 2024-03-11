using Guppy.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    [AutoLoad]
    internal sealed class HelmEngine : BasicEngine,
        IEventEngine<Helm_SetDirection>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;
        private EntityId _dirtyHelmId;

        public HelmEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
        }

        public void Process(VhId vhid, Helm_SetDirection data)
        {
            EntityId id = _entities.GetId(data.ShipVhId);
            ref Helm helm = ref _entities.QueryById<Helm>(id);

            if (data.Value)
            {
                helm.Direction |= data.Which;
            }
            else
            {
                helm.Direction &= ~data.Which;
            }
        }
    }
}
