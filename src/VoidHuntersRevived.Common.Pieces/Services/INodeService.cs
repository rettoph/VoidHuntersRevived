using VoidHuntersRevived.Common.Pieces.Components.Instance;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface INodeService
    {
        bool IsHead(in Node node);

        ref Tree GetTree(in Node node);
    }
}
