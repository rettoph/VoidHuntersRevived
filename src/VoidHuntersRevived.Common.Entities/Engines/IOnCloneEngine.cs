using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IOnCloneEngine<T>
        where T : struct, IEntityComponent
    {
        void OnCloned(in IdMap sourceId, in IdMap cloneId, ref T clone);
    }
}
