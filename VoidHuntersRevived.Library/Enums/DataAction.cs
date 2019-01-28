using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    public enum DataAction
    {
        Configure, // Used to contain basic world settings (walls, gravity, ect...)
        Create, // Used to tell a client to create a new entity
        Update, // Used to tell a client to update an existing entity
        Delete // Used to tell a client to delete an existing entity (should be followed by the entity id)
    }
}
