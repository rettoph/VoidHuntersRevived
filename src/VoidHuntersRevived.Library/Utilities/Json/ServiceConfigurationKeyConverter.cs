using Guppy;
using Guppy.DependencyInjection;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoidHuntersRevived.Library.Utilities.Json
{
    // public class ServiceConfigurationKeyConverter : JsonConverter<ServiceConfigurationKey>
    // {
    //     public override ServiceConfigurationKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //     {
    //         var name = reader.GetString();
    //         var type = reader.GetString();
    //         return new ServiceConfigurationKey(
    //             name: name,
    //             type: GuppyLoader.AssemblyHelper.Types.First(t => t.FullName == type));
    //     }
    // 
    //     public override void Write(Utf8JsonWriter writer, ServiceConfigurationKey value, JsonSerializerOptions options)
    //     {
    //         writer.WriteStartObject();
    //         writer.WriteString("Name", value.Name);
    //         writer.WriteString("Type", value.Type.FullName);
    //         writer.WriteEndObject();
    //     }
    // }
}
