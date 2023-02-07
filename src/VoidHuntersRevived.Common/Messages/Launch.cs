﻿using Guppy;
using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Messages
{
    public class Launch : Message<Launch>
    {
    }

    public class Launch<TGuppy> : Launch
        where TGuppy : IGuppy
    {
        public static readonly Launch<TGuppy> Instance = new Launch<TGuppy>();
    }
}
