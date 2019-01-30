﻿using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Interfaces
{
    public interface IPlayer : INetworkEntity
    {
        String Name { get; }
        TractorBeam TractorBeam { get; }
    }
}
