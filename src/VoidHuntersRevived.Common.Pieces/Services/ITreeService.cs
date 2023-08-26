using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface ITreeService
    {
        ref Node GetHead(in Tree tree);
        ref Node GetHead(in EntityId treeId);
    }
}
