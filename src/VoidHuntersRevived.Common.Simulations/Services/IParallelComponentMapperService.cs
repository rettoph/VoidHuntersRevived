using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IParallelComponentMapperService
    {
        IParallelComponentMapper<T> GetMapper<T>()
            where T : class;
    }
}
