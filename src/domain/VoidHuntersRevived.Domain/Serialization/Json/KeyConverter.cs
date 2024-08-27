using Guppy.Core.Serialization.Common.Services;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    public class KeyConverter : JsonConverter<object>
    {
        private readonly IPolymorphicJsonSerializerService<object> _polymorphicJsonSerializerService;

        public KeyConverter(IPolymorphicJsonSerializerService<object> polymorphicJsonSerializerService)
        {
            _polymorphicJsonSerializerService = polymorphicJsonSerializerService;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.ImplementsGenericTypeDefinition(typeof(Key<>)))
            {
                return true;
            }

            return base.CanConvert(typeToConvert);
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckToken(JsonTokenType.String, true);
            string name = reader.ReadString();
            object key = typeToConvert.GetMethod(nameof(Key<object>.GetByName), BindingFlags.Static | BindingFlags.Public)!.Invoke(null, [name])!;

            return key;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
