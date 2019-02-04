using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface IEntity : ILayerObject
    {
        EntityInfo Info { get; }

        Boolean IsDeleted { get; }

        event EventHandler<IEntity> OnDeleted;


        // Mark the current entity for deletion
        Boolean Delete();
    }
}
