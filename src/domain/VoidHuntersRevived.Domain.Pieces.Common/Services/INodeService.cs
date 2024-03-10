using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.Services
{
    public interface INodeService
    {
        bool IsHead(in Node node);

        ref Tree GetTree(in Node node);
    }
}
