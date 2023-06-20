using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class VoidHuntersEntityDescriptor : ExtendibleEntityDescriptor<BaseEntityDescriptor>
    {
        protected VoidHuntersEntityDescriptor(params IComponentBuilder[] extraComponents) : base(extraComponents)
        {

        }
    }
}
