using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Interfaces;
using Guppy.ServiceLoaders;
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
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterSetup<ColorService>()
                .SetMethod((colors, _, _) =>
                {
                    colors.TryRegister(Colors.ShipPartHullColor, Color.Orange);
                    colors.TryRegister(Colors.ShipPartThrusterColor, Color.Green);
                });
        }
    }
}
