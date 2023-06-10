﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Common.Physics
{
    /// <summary>
    /// Called for each fixture found in the query.
    /// <returns>true: Continues the query, false: Terminate the query</returns>
    /// </summary>
    public delegate bool QueryReportFixtureDelegate(IFixture fixture);

    public interface ISpace
    {
        IBody CreateBody(EntityId id);
        void AddBody(IBody body);
        void RemoveBody(IBody body);

        void QueryAABB(QueryReportFixtureDelegate callback, ref AABB aabb);
        void Step(TimeSpan elapsedGameTime);
    }
}
