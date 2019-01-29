using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface IGameObject : IDrawable, IUpdateable, IInitializable
    {
        IGame Game { get; }

        void SetEnabled(Boolean enabled);
        void SetVisible(Boolean visible);
    }
}
