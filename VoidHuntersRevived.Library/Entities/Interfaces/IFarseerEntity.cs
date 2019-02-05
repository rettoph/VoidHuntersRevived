using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Interfaces
{
    public interface IFarseerEntity
    {
        Body Body { get; }
    }
}
