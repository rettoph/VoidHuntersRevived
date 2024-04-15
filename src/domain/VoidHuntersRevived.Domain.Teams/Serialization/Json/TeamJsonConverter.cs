using Guppy.Core.Resources.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class TeamJsonConverter : JsonConverter<Team>
    {
        public override Team Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Resource<string> name = default!;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Team.Name):
                        string nameKey = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        name = Resource<string>.Get(nameKey);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Team(name);
        }

        public override void Write(Utf8JsonWriter writer, Team value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
