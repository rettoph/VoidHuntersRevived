using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Loaders;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed class PiecePropertyService : IPiecePropertyService
    {
        private readonly Dictionary<Type, PiecePropertyConfiguration> _configurations;
        private readonly List<IPieceProperty> _propertyInstanceCache;

        public PiecePropertyService(ISorted<IPiecePropertyLoader> loaders)
        {
            _configurations = new Dictionary<Type, PiecePropertyConfiguration>();
            _propertyInstanceCache = new List<IPieceProperty>();

            foreach (IPiecePropertyLoader loader in loaders)
            {
                loader.Configure(this);
            }
        }

        public void Configure<T>(Action<IPiecePropertyConfiguration<T>> configuration) 
            where T : class, IPieceProperty
        {
            configuration(this.Get<T>());
        }

        public PiecePropertyConfiguration<T> Get<T>()
            where T : class, IPieceProperty
        {
            if (_configurations.TryGetValue(typeof(T), out PiecePropertyConfiguration? uncasted))
            {
                return Unsafe.As<PiecePropertyConfiguration<T>>(uncasted);
            }

            PiecePropertyConfiguration<T> conf = new PiecePropertyConfiguration<T>();
            _configurations[typeof(T)] = conf;
            return conf;
        }

        public void Initialize(PieceProperty property, IEntityInitializer entity)
        {
            if(!_configurations.TryGetValue(property.GetType(), out PiecePropertyConfiguration? configuration))
            {
                return;
            }

            configuration.Initialize(property, entity);
        }

        public PieceProperty<T> Cache<T>(T property)
            where T : class, IPieceProperty
        {
            _propertyInstanceCache.Add(property);

            return new PieceProperty<T>(property, new PiecePropertyId<T>()
            {
                Value = _propertyInstanceCache.Count - 1
            });
        }
    }
}
