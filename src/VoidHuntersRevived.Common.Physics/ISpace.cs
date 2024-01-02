using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Core;
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
        int BodyCount { get; }
        int ContactCount { get; }

        event OnEventDelegate<IBody> OnBodyEnabled;
        event OnEventDelegate<IBody> OnBodyDisabled;
        event OnEventDelegate<IBody> OnBodyAwakeChanged;

        void EnableBody(in EntityId id);
        void DisableBody(in EntityId id);

        IBody GetBody(in EntityId id);
        IEnumerable<IBody> AllBodies();
        bool TryGetBody(in EntityId id, [MaybeNullWhen(false)] out IBody body);

        void QueryAABB(QueryReportFixtureDelegate callback, ref AABB aabb);
        void Step(Step step);
    }
}
