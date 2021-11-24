using Guppy;
using Guppy.Extensions.System;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Extensions.System.Text.Json;

namespace VoidHuntersRevived.Library.Json.JsonConverters.Contexts.ShipParts
{
    public class ShipPartContextJsonConverter : JsonConverter<ShipPartContext>
    {
        #region Private Enums
        private enum Properties
        {
            ShipPartType,
            ShipPartData
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// A private list of all possible types the current converter might return.
        /// </summary>
        private static Dictionary<String, Type> ShipPartDtoTypes;
        #endregion

        #region Constructors
        static ShipPartContextJsonConverter()
        {
            using(AssemblyHelper assemblyHelper = new AssemblyHelper(withAssembliesReferencing: new[] { typeof(ShipPartContextTypeAttribute).Assembly }))
            {
                ShipPartContextJsonConverter.ShipPartDtoTypes = assemblyHelper.Types
                    .GetTypesWithAttribute<ShipPartContext, ShipPartContextTypeAttribute>()
                    .ToDictionary(
                        keySelector: t => t.GetCustomAttribute<ShipPartContextTypeAttribute>().Name,
                        elementSelector: t => t);
            }
        }
        #endregion

        #region JsonConverter<ShipPartDto> Implementation
        public override bool CanConvert(Type type)
        {
            return type == typeof(ShipPartContext);
        }

        public override ShipPartContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckToken(JsonTokenType.StartObject);
            reader.Read();

            reader.CheckProperty(Properties.ShipPartType);
            reader.Read();

            Type dtoType = ShipPartContextJsonConverter.ShipPartDtoTypes[reader.ReadString()];

            reader.CheckProperty(Properties.ShipPartData);
            reader.Read();

            ShipPartContext instance = this.InnerRead(ref reader, dtoType, options);
            reader.Read();

            reader.CheckToken(JsonTokenType.EndObject);

            return instance;
        }

        public override void Write(Utf8JsonWriter writer, ShipPartContext value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(Properties.ShipPartType, value.GetType().GetCustomAttribute<ShipPartContextTypeAttribute>().Name);

            writer.WritePropertyName(Properties.ShipPartData);
            this.InnerWrite(writer, value, value.GetType(), options);

            writer.WriteEndObject();
        }

        struct hello
        {
            public uint parse() => 1;

        }
        #endregion

        #region Helper Methods
        protected virtual ShipPartContext InnerRead(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonSerializer.Deserialize(ref reader, typeToConvert, options) as ShipPartContext;

        protected virtual void InnerWrite(Utf8JsonWriter writer, ShipPartContext value, Type inputType, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, inputType, options);
        #endregion
    }
}
