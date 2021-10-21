using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    public class ColorsServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterSetup<ColorService>((colors, _, _) =>
            {
                colors.TryRegister(Constants.Colors.ShipPartHullColor, Color.Orange);
                colors.TryRegister(Constants.Colors.ShipPartThrusterColor, Color.Green);
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
