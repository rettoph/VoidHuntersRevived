using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace VoidHuntersRevived.Library.Extensions.System.Text.Json
{
    public static class Utf8JsonReaderExtensions
    {
        public static Boolean CheckPropertyName(ref this Utf8JsonReader reader, String value, Boolean required = true)
        {
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != value)
            {
                if(required)
                    throw new JsonException();

                return false;
            }

            return true;
        }

        public static Boolean CheckProperty<T>(ref this Utf8JsonReader reader, T property, Boolean required = true)
            where T : Enum
        {
            return reader.CheckPropertyName(property.ToString(), required);
        }

        public static Boolean CheckToken(ref this Utf8JsonReader reader, JsonTokenType tokenType, Boolean required = true)
        {
            if (reader.TokenType != tokenType)
            {
                if (required)
                    throw new JsonException();

                return false;
            }

            return true;
        }

        public static Boolean ReadPropertyName(ref this Utf8JsonReader reader, out String propertyName)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                propertyName = default;
                return false;
            }

            propertyName = reader.GetString();
            reader.Read();

            return true;
        }

        public static Boolean ReadProperty<T>(ref this Utf8JsonReader reader, out T property)
            where T : Enum
        {
            if(reader.ReadPropertyName(out String propertyName))
            {
                property = (T)Enum.Parse(typeof(T), propertyName);
                return true;
            }

            property = default;
            return false;
        }

        public static UInt32 ReadUInt32(ref this Utf8JsonReader reader)
        {
            reader.CheckToken(JsonTokenType.Number);
            UInt32 value = reader.GetUInt32();
            reader.Read();

            return value;
        }

        public static Single ReadSingle(ref this Utf8JsonReader reader)
        {
            reader.CheckToken(JsonTokenType.Number);
            Single value = reader.GetSingle();
            reader.Read();

            return value;
        }

        public static String ReadString(ref this Utf8JsonReader reader)
        {
            reader.CheckToken(JsonTokenType.String);
            String value = reader.GetString();
            reader.Read();

            return value;
        }
    }
}
