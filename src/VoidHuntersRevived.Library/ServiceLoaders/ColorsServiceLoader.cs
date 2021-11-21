using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    public class ColorsServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterSetup<ColorService>((colors, _, _) =>
            {
                colors.TryRegister(Colors.ShipPartHullColor, Color.Orange);
                colors.TryRegister(Colors.ShipPartThrusterColor, Color.Green);
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
