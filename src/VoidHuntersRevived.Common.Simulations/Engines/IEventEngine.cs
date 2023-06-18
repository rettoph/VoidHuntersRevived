﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    public interface IEventEngine : IEngine
    {
    }

    public interface IEventEngine<T> : IEventEngine
        where T : IEventData
    {
        void Process(VhId eventId, T data);
    }
}
