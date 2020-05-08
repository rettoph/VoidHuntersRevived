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
    public enum GameAuthorization
    {
        None    = 0, // Indicates that all permanent game actions must be authorized directly from the connected peer.
        Partial = 1, // Indicates that some permanent game actions may be done locally before confirmation.
        Full    = 2 // Indicates that all permanent game actions may be done locally without any confirmation.
    }
}
