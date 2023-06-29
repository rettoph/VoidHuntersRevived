using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Game.Events;
using VoidHuntersRevived.Game.Pieces.Components;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class EntityEngine : BasicEngine,
        IEventEngine<TryCreateEntity>,
        IRevertEventEngine<TryCreateEntity>,
        IEventEngine<TryDestroyEntity>,
        IRevertEventEngine<TryDestroyEntity>,
        IStepEngine<Step>
    {
        private readonly HashCache<VhId> _destroyedEntities;
        private readonly ILogger _logger;

        public string name { get; } = nameof(EntityEngine);

        public EntityEngine(ILogger logger)
        {
            _logger = logger;
            _destroyedEntities = new HashCache<VhId>(TimeSpan.FromSeconds(10));
        }

        public void Step(in Step _param)
        {
            _destroyedEntities.Prune();
        }

        public void Process(VhId eventId, TryCreateEntity data)
        {
            this.Simulation.Entities.Create(data.Type, data.EntityVhId, data.Initializer);
        }

        public void Revert(VhId eventId, TryCreateEntity data)
        {
            this.Simulation.Entities.Destroy(data.EntityVhId);
        }

        public void Process(VhId eventId, TryDestroyEntity data)
        {
            this.Simulation.Entities.Destroy(data.EntityVhId);
        }

        public void Revert(VhId eventId, TryDestroyEntity data)
        {
            this.Simulation.Entities.Serialization.Deserialize(data.Backup);
        }
    }
}
