using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IValidEventEngine : IEngine
    {
    }

    public interface IVerifyEventEngine<T> : IValidEventEngine
        where T : IEventData
    {
        void Validate(VhId eventId, T data);
    }
}
