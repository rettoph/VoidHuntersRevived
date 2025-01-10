using System.Text.Json;
using System.Text.Json.Serialization;
using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json
{
    public class ResourceComponentConverter(IResourceService resourceService) : JsonConverter<object>
    {

        private readonly IResourceService _resourceService = resourceService;

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsGenericType == false)
            {
                return false;
            }

            bool result = typeToConvert.GetGenericTypeDefinition() == typeof(ResourceComponent<>);
            return result;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckToken(JsonTokenType.String, true);
            string key = reader.ReadString();

            object? instance = Activator.CreateInstance(typeToConvert, [key, this._resourceService]);

            return instance ?? throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}