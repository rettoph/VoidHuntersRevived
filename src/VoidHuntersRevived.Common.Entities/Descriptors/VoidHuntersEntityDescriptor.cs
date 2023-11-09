using Guppy.Resources;
using Microsoft.Xna.Framework;
using Standart.Hash.xxHash;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : IDynamicEntityDescriptor, IEntityResource<VoidHuntersEntityDescriptor>
    {
        private DynamicEntityDescriptor<BaseEntityDescriptor> _dynamicDescriptor;
        private readonly List<ComponentManager> _componentManagers;
        private EntityInitializerDelegate? _postInitializer;

        private Id<VoidHuntersEntityDescriptor>? _id;
        private string? _name;
        public unsafe Id<VoidHuntersEntityDescriptor> Id
        {
            get
            {
                if(_id is null)
                {
                    uint128 nameHash = xxHash128.ComputeHash(this.GetType().AssemblyQualifiedName);
                    VhId* pNameHash = (VhId*)&nameHash;

                    _id = new Id<VoidHuntersEntityDescriptor>(NameSpace<VoidHuntersEntityDescriptor>.Instance.Create(pNameHash[0]));
                }
                return _id.Value;
            }
        }
        public string Name => _name ??= this.GetType().Name;
        public Resource<Color> PrimaryColor { get; } = Resources.Colors.None;
        public Resource<Color> SecondaryColor { get; } = Resources.Colors.None;
        public int Order { get; } = 0;

        public IComponentBuilder[] componentsToBuild => _dynamicDescriptor.componentsToBuild;

        public IEnumerable<ComponentManager> ComponentManagers => _componentManagers;

        protected VoidHuntersEntityDescriptor() : this(Resources.Colors.None, Resources.Colors.None, 0)
        {

        }
        protected unsafe VoidHuntersEntityDescriptor(Resource<Color> primaryColor, Resource<Color> secondaryColor, int order)
        {
            _dynamicDescriptor = DynamicEntityDescriptor<BaseEntityDescriptor>.CreateDynamicEntityDescriptor();
            _componentManagers = new List<ComponentManager>();

            this.PrimaryColor = primaryColor;
            this.SecondaryColor = secondaryColor;
            this.Order = order;
        }

        protected VoidHuntersEntityDescriptor ExtendWith(ComponentManager[] managers)
        {
            var builders = managers.Select(x => x.Builder).ToArray();
            _dynamicDescriptor.ExtendWith(builders);

            foreach(ComponentManager manager in managers)
            {
                _componentManagers.Add(manager);
            }

            return this;
        }

        protected VoidHuntersEntityDescriptor WithPostInitializer(EntityInitializerDelegate initializer)
        {
            _postInitializer += initializer;

            return this;
        }

        public bool HasAll(params Type[] componentTypes)
        {
            foreach(Type componentType in componentTypes)
            {
                if(this.ComponentManagers.Any(x => x.Type == componentType) == false)
                {
                    return false;
                }
            }

            return true;
        }

        internal void PostInitialize(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            _postInitializer?.Invoke(entities, ref initializer, in id);
        }
    }
}
