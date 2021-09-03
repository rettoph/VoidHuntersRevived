using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace VoidHuntersRevived.Library.Extensions.System.Text.Json
{
    public static class Utf8JsonWriterExtensions
    {
        public static void WritePropertyName(this Utf8JsonWriter writer, Enum property)
        {
            writer.WritePropertyName(property.ToString());
        }

        public static void WriteString(this Utf8JsonWriter writer, Enum property, String value)
        {
            writer.WriteString(property.ToString(), value);
        }

        public static void WriteNumber(this Utf8JsonWriter writer, Enum property, UInt32 value)
        {
            writer.WriteNumber(property.ToString(), value);
        }

        public static void WriteNumber(this Utf8JsonWriter writer, Enum property, Single value)
        {
            writer.WriteNumber(property.ToString(), value);
        }
    }
}
