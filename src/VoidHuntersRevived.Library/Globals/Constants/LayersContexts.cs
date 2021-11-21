using Guppy.Contexts;
using Guppy.LayerGroups;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Globals.Constants
{
    public static class LayersContexts
    {
        public static readonly LayerContext Chunks = new LayerContext()
        {
            Group = new SingleLayerGroup(0)
        };

        public static readonly LayerContext Players = new LayerContext()
        {
            Group = new SingleLayerGroup(1)
        };

        public static readonly LayerContext Ships = new LayerContext()
        {
            Group = new SingleLayerGroup(2)
        };

        public static readonly LayerContext Chains = new LayerContext()
        {
            Group = new SingleLayerGroup(3)
        };
    }
}
