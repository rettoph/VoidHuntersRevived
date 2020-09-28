using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Enums;
using Guppy.IO.Input;
using Guppy.IO.Input.Contexts;
using Guppy.IO.Input.Services;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad(200)]
    internal sealed class InputServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            // throw new NotImplementedException();
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            var inputs = provider.GetService<InputCommandService>();

            inputs.Add(new InputCommandContext()
            {
                Handle = "set_direction_forward",
                DefaultInput = new InputType(Keys.W),
                Command = "set direction -d=forward -v=[state]",
                States = new ButtonState[]
                    {
                        ButtonState.Pressed,
                        ButtonState.Released
                    }
            });
        }
    }
}
