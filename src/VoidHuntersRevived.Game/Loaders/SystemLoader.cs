﻿using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    internal sealed class SystemLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
