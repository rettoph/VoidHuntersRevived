using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Structs
{
    public struct EntityInfo
    {
        public readonly String Handle;
        public readonly Type Type;
        public readonly String Name;
        public readonly String Description;
        public readonly Object Data;

        public EntityInfo(String handle, Type type, String name, String description, Object data = null)
        {
            this.Handle = handle;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Data = data;
        }
    }
}
