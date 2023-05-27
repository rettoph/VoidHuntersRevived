using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed class ParallelComponentMapperService : IParallelComponentMapperService
    {
        private World _world;
        private readonly Dictionary<Type, IParallelComponentMapper> _mappers;

        public ParallelComponentMapperService(World world)
        {
            _world = world;
            _mappers = new Dictionary<Type, IParallelComponentMapper>();
        }

        public IParallelComponentMapper<T> GetMapper<T>()
            where T : class
        {
            if (_mappers.TryGetValue(typeof(T), out IParallelComponentMapper? cached))
            {
                return Unsafe.As<IParallelComponentMapper<T>>(cached);
            }

            IParallelComponentMapper<T> mapper = new ParallelComponentMapper<T>(_world.ComponentManager.GetMapper<T>());
            _mappers.Add(typeof(T), mapper);

            return mapper;
        }
    }
}
