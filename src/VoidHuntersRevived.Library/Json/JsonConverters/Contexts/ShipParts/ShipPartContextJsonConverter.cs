using Guppy.Extensions.System;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Library.Contexts.ShipParts;

namespace VoidHuntersRevived.Library.Json.JsonConverters.Contexts.ShipParts
{
    public class ShipPartContextJsonConverter : JsonConverter<ShipPartContext>
    {
        #region Private Fields
        /// <summary>
        /// A private list of all possible types the current converter might return.
        /// </summary>
        private static Dictionary<UInt32, Type> ShipPartDtoTypes;
        #endregion

        #region Constructors
        static ShipPartContextJsonConverter()
        {
            ShipPartContextJsonConverter.ShipPartDtoTypes = AssemblyHelper.Types
                .GetTypesAssignableFrom<ShipPartContext>()
                .ToDictionary(
                    keySelector: t => t.AssemblyQualifiedName.xxHash(),
                    elementSelector: t => t);
        }
        #endregion

        #region JsonConverter<ShipPartDto> Implementation
        public override bool CanConvert(Type type)
        {
            return typeof(ShipPartContext).IsAssignableFrom(type);
        }

        public override ShipPartContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != "Discriminator")
            {
                throw new JsonException();
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            Type dtoType = ShipPartContextJsonConverter.ShipPartDtoTypes[reader.GetUInt32()];

            if (!reader.Read() || reader.GetString() != "Data")
            {
                throw new JsonException();
            }
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            ShipPartContext instance = this.InnerRead(ref reader, dtoType, options);

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return instance;
        }

        public override void Write(Utf8JsonWriter writer, ShipPartContext value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("Discriminator", value.GetType().AssemblyQualifiedName.xxHash());

            writer.WritePropertyName("Data");
            this.InnerWrite(writer, value, options);

            writer.WriteEndObject();
        }
        #endregion

        #region Helper Methods
        protected virtual ShipPartContext InnerRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonSerializer.Deserialize(ref reader, typeToConvert) as ShipPartContext;

        protected virtual void InnerWrite(Utf8JsonWriter writer, ShipPartContext value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value);
        #endregion
    }
}
