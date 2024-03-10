using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json
{
    internal class VoidHuntersEntityDescriptorConverter : JsonConverter<VoidHuntersEntityDescriptor>
    {
        private readonly Dictionary<string, VoidHuntersEntityDescriptor> _descriptors;

        public VoidHuntersEntityDescriptorConverter(IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _descriptors = descriptors.ToDictionary(x => x.Name, x => x);
        }
        public override VoidHuntersEntityDescriptor? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string name = reader.GetString() ?? string.Empty;

            if (_descriptors.TryGetValue(name, out var descriptor))
            {
                return descriptor;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, VoidHuntersEntityDescriptor value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
