using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IInput<TData> : IEvent<TData> 
        where TData : IData
    {
        int UserId { get; }
    }
}
