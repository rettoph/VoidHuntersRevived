using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Physics.Common
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

        void EnableBody(in EntityLocalId entityLocalId);
        void DisableBody(in EntityLocalId entityLocalId);

        IBody GetBody(in EntityLocalId entityLocalId);
        IEnumerable<IBody> AllBodies();
        bool TryGetBody(in EntityLocalId entityLocalId, [MaybeNullWhen(false)] out IBody body);

        void QueryAABB(QueryReportFixtureDelegate callback, ref AABB aabb);
        void Step(Step step);
    }
}
