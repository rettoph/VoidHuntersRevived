using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.ECS
{
    public interface IComponentMapper<T1>
        where T1 : struct
    {

    }

    public interface IComponentMapper<T1, T2>
        where T1 : struct
        where T2 : struct
    {
    }
}
