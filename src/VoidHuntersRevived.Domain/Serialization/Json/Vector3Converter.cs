using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    internal class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Vector3 result = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Vector3.X):
                        result.X = reader.ReadSingle();
                        break;
                    case nameof(Vector3.Y):
                        result.Y = reader.ReadSingle();
                        break;
                    case nameof(Vector3.Z):
                        result.Z = reader.ReadSingle();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
