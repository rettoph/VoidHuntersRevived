using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    public struct UserId(int value) : IEntityComponent
    {
        public int? Value { get; internal set; } = value;
    }
}