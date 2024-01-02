using Guppy.Attributes;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Ships.Events;

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
