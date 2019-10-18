using Guppy;
using Guppy.Configurations;
using Guppy.Loaders;
using Guppy.Utilities;
using Guppy.Utilities.Loggers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library;
using VoidHuntersRevived.Client.Textures.Attributes;
using VoidHuntersRevived.Client.Textures.Builders;

namespace VoidHuntersRevived.Client.Textures
{
    class Program
    {
        static void Main(string[] args)
        {
            var guppy = new GuppyLoader();
            var provider = guppy.ConfigureLogger<ConsoleLogger>()
                .Initialize()
                .BuildServiceProvider();
            var entities = provider.GetRequiredService<EntityLoader>();
            var builders = AssemblyHelper.GetTypesWithAttribute<Builder, IsBuilderAttribute>().ToDictionary(
                keySelector: t => t.GetCustomAttributes(true).Where(attr => attr is IsBuilderAttribute).Select(attr => attr as IsBuilderAttribute).OrderBy(attr => attr.Priority).First().Type,
                elementSelector: t => ActivatorUtilities.CreateInstance(provider, t) as Builder);

            if (!Directory.Exists("Sprites")) // Create a new directory 
                Directory.CreateDirectory("Sprites");

            foreach (KeyValuePair<String, EntityConfiguration> entity in entities)
                foreach (KeyValuePair<Type, Builder> builder in builders)
                    if (builder.Key.IsAssignableFrom(entity.Value.Type))
                        builder.Value.TryBuild(entity.Key, entity.Value);

            Console.ReadLine();
        }
    }
}
