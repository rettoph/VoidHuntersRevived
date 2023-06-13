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
using VoidHuntersRevived.Common.Entities;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public sealed class PiecePropertyService : IPiecePropertyService
    {
        private readonly Dictionary<Type, PiecePropertyConfiguration> _configurations;
        private readonly List<IPieceProperty> _propertyInstanceCache;

        public PiecePropertyService(PieceCategoryService categories, ISorted<IPiecePropertyLoader> loaders)
        {
            _configurations = new Dictionary<Type, PiecePropertyConfiguration>();
            _propertyInstanceCache = new List<IPieceProperty>();

            Type[] propertyTypes = categories.Configurations.SelectMany(x => x.PropertyTypes).Distinct().ToArray();
            foreach(Type propertyType in propertyTypes)
            {
                Type configurationType = typeof(PiecePropertyConfiguration<>).MakeGenericType(propertyType);
                PiecePropertyConfiguration configuration = (PiecePropertyConfiguration)Activator.CreateInstance(configurationType)!;

                _configurations.Add(configuration.Type, configuration);
            }

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
            return Unsafe.As<PiecePropertyConfiguration<T>>(_configurations[typeof(T)]);
        }

        public void Initialize(PieceProperty property, ref EntityInitializer entity)
        {
            if(!_configurations.TryGetValue(property.Type, out PiecePropertyConfiguration? configuration))
            {
                return;
            }

            configuration.Initialize(property, ref entity);
        }

        public PieceProperty<T> Cache<T>(T property)
            where T : class, IPieceProperty
        {
            _propertyInstanceCache.Add(property);

            return new PieceProperty<T>(property, new Piece<T>()
            {
                Value = _propertyInstanceCache.Count + 1
            });
        }

        public T Get<T>(Piece<T> piece)
            where T : class, IPieceProperty
        {
            return Unsafe.As<T>(_propertyInstanceCache[piece.Value]);
        }
    }
}
