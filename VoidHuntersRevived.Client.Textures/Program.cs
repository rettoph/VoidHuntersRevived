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
using VoidHuntersRevived.Client.Textures.Attributes;
using VoidHuntersRevived.Client.Textures.TextureGenerators;

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
            var builders = AssemblyHelper.GetTypesWithAttribute<TextureGenerator, IsTextureGeneratorAttribute>().ToDictionary(
                keySelector: t => t.GetCustomAttributes(true).Where(attr => attr is IsTextureGeneratorAttribute).Select(attr => attr as IsTextureGeneratorAttribute).OrderBy(attr => attr.Priority).First().Type,
                elementSelector: t => ActivatorUtilities.CreateInstance(provider, t) as TextureGenerator);

            if (!Directory.Exists("Sprites")) // Create a new directory 
                Directory.CreateDirectory("Sprites");

            foreach (KeyValuePair<String, EntityConfiguration> entity in entities)
                foreach (KeyValuePair<Type, TextureGenerators.TextureGenerator> builder in builders)
                    if (builder.Key.IsAssignableFrom(entity.Value.Type))
                        builder.Value.TryGenerate(entity.Key, entity.Value);

            // Console.ReadLine();
        }
    }
}
