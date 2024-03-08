using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Pieces;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal sealed class BlueprintConverter : JsonConverter<Blueprint>
    {
        public override Blueprint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // HashBuilder<Blueprint, VhId, VhId>.Instance.CalculateId(VhId.HashString(this.Name), this.Head.Hash)
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Blueprint value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
