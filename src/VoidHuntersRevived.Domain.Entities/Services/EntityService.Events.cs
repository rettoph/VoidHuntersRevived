using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService : 
        IEventEngine<CreateEntity>,
        IRevertEventEngine<CreateEntity>, 
        IEventEngine<DestroyEntity>,
        IRevertEventEngine<DestroyEntity>
    {
        public void Process(VhId eventId, CreateEntity data)
        {
            this.Create(data.Type, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, CreateEntity data)
        {
            this.Destroy(data.VhId);
        }

        public void Process(VhId eventId, DestroyEntity data)
        {
            this.Destroy(data.VhId);
        }

        public void Revert(VhId eventId, DestroyEntity data)
        {
            throw new NotImplementedException();
        }
    }
}
