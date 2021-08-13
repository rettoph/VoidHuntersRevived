using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Input;
using Guppy.IO.Input.Contexts;
using Guppy.IO.Input.Services;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.ServiceLoaders
{
    [AutoLoad]
    internal sealed class InputServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            // throw new NotImplementedException();
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            var inputs = provider.GetService<InputCommandService>();

            #region Lock Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "lock_rotation",
                DefaultInput = new InputType(Keys.LeftShift),
                Commands = new[]
                {
                    (state: ButtonState.Released, command: "lock -t=Rotation -v=true"),
                    (state: ButtonState.Pressed, command: "lock -t=Rotation -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "lock_length",
                DefaultInput = new InputType(Keys.LeftControl),
                Commands = new[]
                {
                    (state: ButtonState.Released, command: "lock -t=Length -v=true"),
                    (state: ButtonState.Pressed, command: "lock -t=Length -v=false")
                }
            });

            inputs.Add(new InputCommandContext()
            {
                Handle = "lock_point_snap",
                DefaultInput = new InputType(Keys.LeftAlt),
                Commands = new[]
                {
                    (state: ButtonState.Released, command: "lock -t=PointSnap -v=true"),
                    (state: ButtonState.Pressed, command: "lock -t=PointSnap -v=false")
                }
            });
            #endregion

            #region Complete Inputs
            inputs.Add(new InputCommandContext()
            {
                Handle = "complete_outer_hull",
                DefaultInput = new InputType(Keys.Space),
                Commands = new[]
                {
                    (state: ButtonState.Released, command: "complete -t=OuterHull")
                }
            });
            #endregion
        }
    }
}
