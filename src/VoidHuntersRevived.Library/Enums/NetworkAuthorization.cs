using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    /// <summary>
    /// Simple Enum used to designate the level of control
    /// a game should exibit when making decisions.
    /// 
    /// An example of this is the Server with full authorization will
    /// destroy a ship at zero health, while a client with partial
    /// authorization will wait for confirmation from the server before
    /// destroying the same ship.
    /// </summary>
    public enum NetworkAuthorization
    {
        Slave,
        Master
    }
}
