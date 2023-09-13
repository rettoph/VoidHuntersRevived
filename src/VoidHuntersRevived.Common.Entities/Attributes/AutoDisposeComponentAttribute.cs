﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = true)]
    public class AutoDisposeComponentAttribute : Attribute
    {
        public readonly Type[] GenericParameters;
        public readonly AutoDisposeScope Scope;

        internal AutoDisposeComponentAttribute(AutoDisposeScope scope) : this(Array.Empty<Type>(), scope)
        {

        }
        internal AutoDisposeComponentAttribute(Type[] genericParameters, AutoDisposeScope scope)
        {
            this.GenericParameters = genericParameters;
            this.Scope = scope;
        }

        public Type GetDisposableComponentType(Type componentType)
        {
            if(this.GenericParameters.Length == 0)
            {
                return componentType;
            }

            if(componentType.IsGenericType)
            {
                componentType = componentType.GetGenericTypeDefinition();
            }

            return componentType.MakeGenericType(this.GenericParameters);
        }
    }
    public class AutoDisposeComponentAttribute<T> : AutoDisposeComponentAttribute
    {
        public AutoDisposeComponentAttribute(AutoDisposeScope scope) : base(new[] { typeof(T) }, scope)
        {

        }
    }
}
