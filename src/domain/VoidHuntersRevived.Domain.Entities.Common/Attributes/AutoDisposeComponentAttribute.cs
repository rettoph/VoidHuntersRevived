using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Attributes
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
            if (this.GenericParameters.Length == 0)
            {
                return componentType;
            }

            if (componentType.IsGenericType)
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
