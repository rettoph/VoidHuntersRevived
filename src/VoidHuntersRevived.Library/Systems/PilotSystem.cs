using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Extensions;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Messages.Inputs;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoLoad]
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class PilotSystem : EntitySystem, ISubscriber<PilotDirectionInput>
    {
        private IBus _bus;
        private PilotIdMap _pilotMap;
        private ComponentMapper<Piloting> _pilotingMapper;

        public PilotSystem(PilotIdMap pilotIdMap, IBus bus) : base(Aspect.All(typeof(Piloting)))
        {
            _bus = bus;
            _pilotMap = pilotIdMap;
            _pilotingMapper = default!;

            _bus.SubscribeAll(this);
        }

        ~PilotSystem()
        {
            _bus.Unsubscribe(this);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotingMapper = mapperService.GetMapper<Piloting>();
        }

        public void Process(in PilotDirectionInput message)
        {
            var entityId = _pilotMap.GetEntityId(message.PilotId);

            var piloting = _pilotingMapper.Get(entityId);

            piloting.SetDirection(message);

            Console.WriteLine(piloting.Direction);
        }
    }
}
