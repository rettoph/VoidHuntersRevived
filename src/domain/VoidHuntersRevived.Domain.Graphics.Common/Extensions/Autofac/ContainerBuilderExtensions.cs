using Autofac;
using Guppy.Core.Resources.Serialization.Json;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common.Extensions.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterPrimitiveType<TVertexInstance, TVertexStatic, TEffect>(this ContainerBuilder builder, string name)
            where TVertexInstance : unmanaged, IVertexType
            where TVertexStatic : unmanaged, IVertexType
            where TEffect : Effect
        {
            var polymorphicMap = new PolymorphicJsonType<PrimitiveType<TVertexInstance, TVertexStatic, TEffect>, PrimitiveType>(name);
            builder.RegisterInstance<PolymorphicJsonType>(polymorphicMap).SingleInstance();
        }
    }
}
