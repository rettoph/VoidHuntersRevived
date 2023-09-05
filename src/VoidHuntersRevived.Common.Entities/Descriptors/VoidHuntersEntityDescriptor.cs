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
    public abstract class VoidHuntersEntityDescriptor : IDynamicEntityDescriptor
    {
        private DynamicEntityDescriptor<BaseEntityDescriptor> _dynamicDescriptor;
        private readonly List<ComponentManager> _componentManagers;

        private VhId? _id;
        private string? _name;
        public unsafe VhId Id
        {
            get
            {
                if(_id is null)
                {
                    uint128 nameHash = xxHash128.ComputeHash(this.GetType().AssemblyQualifiedName);
                    VhId* pNameHash = (VhId*)&nameHash;

                    _id = NameSpace<VoidHuntersEntityDescriptor>.Instance.Create(pNameHash[0]);
                }
                return _id.Value;
            }
        }
        public string Name => _name ??= this.GetType().Name;
        public Resource<Color> DefaultColor { get; }

        public IComponentBuilder[] componentsToBuild => _dynamicDescriptor.componentsToBuild;

        public IEnumerable<ComponentManager> ComponentManagers => _componentManagers;

        protected VoidHuntersEntityDescriptor() : this(Resources.Colors.None)
        {

        }
        protected unsafe VoidHuntersEntityDescriptor(Resource<Color> defaultColor)
        {
            _dynamicDescriptor = DynamicEntityDescriptor<BaseEntityDescriptor>.CreateDynamicEntityDescriptor();
            _componentManagers = new List<ComponentManager>();

            this.DefaultColor = defaultColor;
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
    }
}
