using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
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
        event OnEventDelegate<IBody> OnBodyEnabled;
        event OnEventDelegate<IBody> OnBodyDisabled;
        event OnEventDelegate<IBody> OnBodyAwakeChanged;

        IBody GetOrCreateBody(in EntityId id);
        void DestroyBody(in EntityId id);
        IBody GetBody(in EntityId id);
        IEnumerable<IBody> AllBodies();
        bool TryGetBody(in EntityId id, [MaybeNullWhen(false)] out IBody body);

        void QueryAABB(QueryReportFixtureDelegate callback, ref AABB aabb);
        void Step(Step step);
    }
}
