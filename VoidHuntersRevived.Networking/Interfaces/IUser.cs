using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Networking.Interfaces
{
    public interface IUser
    {
        Int32 Id { get; }
        String Name { get; }
    }
}
