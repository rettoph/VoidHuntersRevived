using Autofac;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Contexts;
using BufferUsage = Microsoft.Xna.Framework.Graphics.BufferUsage;
using Effect = Microsoft.Xna.Framework.Graphics.Effect;
using GraphicsDevice = Microsoft.Xna.Framework.Graphics.GraphicsDevice;
using IndexBuffer = Microsoft.Xna.Framework.Graphics.IndexBuffer;
using IndexElementSize = Microsoft.Xna.Framework.Graphics.IndexElementSize;
using IVertexType = Microsoft.Xna.Framework.Graphics.IVertexType;
using VertexBuffer = Microsoft.Xna.Framework.Graphics.VertexBuffer;

namespace VoidHuntersRevived.Domain.Graphics.Serialization.Json
{
    public class PrimitiveTypeConverter(ILifetimeScope scope, GraphicsDevice? graphics = null) : JsonConverter<object>
    {
        private readonly ILifetimeScope _scope = scope;
        private readonly GraphicsDevice? _graphics = graphics;

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsAssignableTo<IPrimitiveType>() == false)
            {
                return false;
            }

            if (typeToConvert.IsGenericType == false)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != typeof(IPrimitiveType<,,>))
            {
                return false;
            }

            return true;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (_graphics is null)
            {
                reader.Skip();

                Type notImplementedPrimitiveTypeType = typeof(DefaultPrimitiveType<,,>).MakeGenericType(typeToConvert.GenericTypeArguments);
                object? notImplementedInstance = Activator.CreateInstance(notImplementedPrimitiveTypeType);
                return notImplementedInstance ?? throw new NotImplementedException();
            }

            Type instanceVertexType = typeToConvert.GenericTypeArguments[0];
            Type staticVertexType = typeToConvert.GenericTypeArguments[1];
            Type effectType = typeToConvert.GenericTypeArguments[2];

            PrimitiveContextTwo[] primitiveContexts = [];
            Array? vertices = null;
            IndexBufferContext[] indexBufferContexts = [];

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(IPrimitiveType.Primitives):
                        primitiveContexts = JsonSerializer.Deserialize<PrimitiveContextTwo[]>(ref reader, options) ?? [];
                        reader.Read();
                        break;
                    case nameof(IPrimitiveType.VertexBuffer):
                        vertices = (Array?)JsonSerializer.Deserialize(ref reader, staticVertexType.MakeArrayType(), options);
                        reader.Read();
                        break;
                    case nameof(IPrimitiveType.IndexBuffers):
                        indexBufferContexts = JsonSerializer.Deserialize<IndexBufferContext[]>(ref reader, options) ?? [];
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            if (primitiveContexts.Length == 0)
            {
                throw new NotImplementedException();
            }

            if (vertices is null)
            {
                throw new NotImplementedException();
            }

            if (indexBufferContexts.Length == 0)
            {
                throw new NotImplementedException();
            }

            VertexBuffer vertexBuffer = new(_graphics, staticVertexType, vertices.Length, BufferUsage.WriteOnly);
            SetDataMethod.MakeGenericMethod(staticVertexType).Invoke(null, [vertexBuffer, vertices]);

            IndexBuffer[] indexBuffers = indexBufferContexts.Select(x =>
            {
                IndexBuffer indexBuffer = new(_graphics, IndexElementSize.SixteenBits, x.Values.Length, BufferUsage.WriteOnly);
                indexBuffer.SetData(x.Values);

                return indexBuffer;
            }).ToArray();

            PrimitiveTypeEnum[] bufferTypes = [.. indexBufferContexts.Select(x => x.Type)];

            Type primitiveType = typeof(Primitive<,,>).MakeGenericType(typeToConvert.GenericTypeArguments);
            Lazy<IPrimitive[]> primitives = new(() =>
            {
                Effect effect = (Effect)_scope.Resolve(effectType);

                return primitiveContexts.Select(x =>
                {
                    IPrimitive? primitive = (IPrimitive?)Activator.CreateInstance(primitiveType, [x.Sequence, x.SequenceGroup, vertexBuffer, indexBuffers, bufferTypes, effect, _graphics]);
                    return primitive ?? throw new NotImplementedException();
                }).ToArray();
            });

            Type primitiveTypeType = typeof(PrimitiveType<,,>).MakeGenericType(typeToConvert.GenericTypeArguments);
            object? instance = Activator.CreateInstance(primitiveTypeType, [primitives, vertexBuffer, indexBuffers, bufferTypes]);

            return instance ?? throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private struct IndexBufferContext
        {
            public PrimitiveTypeEnum Type { get; set; }
            public short[] Values { get; set; }
        }

        private static readonly MethodInfo SetDataMethod = typeof(PrimitiveTypeConverter).GetMethod(nameof(SetData), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new NotImplementedException();
        private static void SetData<TVertex>(VertexBuffer vertexBuffer, TVertex[] data)
            where TVertex : unmanaged, IVertexType
        {
            vertexBuffer.SetData(data);
        }
    }
}
