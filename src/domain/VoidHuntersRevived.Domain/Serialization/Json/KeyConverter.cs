using Guppy.Core.Serialization.Common.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    public class KeyConverter : JsonConverter<IKey>
    {
        private readonly IPolymorphicJsonSerializerService<object> _polymorphicJsonSerializerService;

        public KeyConverter(IPolymorphicJsonSerializerService<object> polymorphicJsonSerializerService)
        {
            _polymorphicJsonSerializerService = polymorphicJsonSerializerService;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsAssignableTo(typeof(IKey)))
            {
                return true;
            }

            return base.CanConvert(typeToConvert);
        }

        public override IKey? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? name = null;
            Type? type = null;

            if (typeToConvert.IsGenericType)
            {
                type ??= typeToConvert.GenericTypeArguments[0];
            }

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    this.ReadString(ref reader, ref type, ref name, options);
                    break;
                case JsonTokenType.StartObject:
                    this.ReadObject(ref reader, ref type, ref name, options);
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (name is null)
            {
                throw new ArgumentException(nameof(IKey.Name));
            }

            if (type is null)
            {
                throw new ArgumentException(nameof(IKey.Type));
            }

            return Key.GetByName(name, type);
        }

        public override void Write(Utf8JsonWriter writer, IKey value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private void ReadObject(ref Utf8JsonReader reader, ref Type? type, ref string? name, JsonSerializerOptions options)
        {
            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(IKey.Name):
                        name = JsonSerializer.Deserialize<string>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(IKey.Type):
                        string typeKey = JsonSerializer.Deserialize<string>(ref reader, options) ?? string.Empty;
                        type = _polymorphicJsonSerializerService.GetType(typeKey);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);
        }

        private void ReadString(ref Utf8JsonReader reader, ref Type? type, ref string? name, JsonSerializerOptions options)
        {
            name = JsonSerializer.Deserialize<string>(ref reader, options);
            reader.Read();
        }
    }
}
