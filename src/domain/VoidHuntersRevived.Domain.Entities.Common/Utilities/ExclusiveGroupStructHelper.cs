﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public static class ExclusiveGroupStructHelper
    {
        private static readonly Dictionary<string, ExclusiveGroup> _groups = new Dictionary<string, ExclusiveGroup>();

        public static ExclusiveGroupStruct GetOrCreateExclusiveStruct(string name)
        {
            ref ExclusiveGroup? group = ref CollectionsMarshal.GetValueRefOrAddDefault(_groups, name, out bool exists);

            if(exists)
            {
                return group!;
            }

            group = new ExclusiveGroup(name);
            return group;
        }
    }
}
