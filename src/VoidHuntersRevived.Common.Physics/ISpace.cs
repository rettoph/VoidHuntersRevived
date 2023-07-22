using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        IBody GetOrCreateBody(in VhId id);
        void DestroyBody(in VhId id);
        IBody GetBody(in VhId id);
        IEnumerable<IBody> AllBodies();
        bool TryGetBody(in VhId id, [MaybeNullWhen(false)] out IBody body);

        void QueryAABB(QueryReportFixtureDelegate callback, ref AABB aabb);
        void Step(Step step);
    }
}
