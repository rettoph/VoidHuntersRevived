using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Scenes.Interfaces
{
    /// <summary>
    /// A scene designed to contain farseer entities
    /// </summary>
    public interface IFarseerScene : IScene
    {
        World World { get; }
        Wall Wall { get; }
    }
}
