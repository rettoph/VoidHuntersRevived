using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
namespace VoidHuntersRevived.Library.Entities.Interfaces
{
    public interface ITractorBeam : IFarseerEntity
    {
        ITractorableEntity SelectedEntity { get; }
        Vector2 Position { get; set; }

        event EventHandler<ITractorBeam> OnSelect;
        event EventHandler<ITractorBeam> OnRelease;

        void TrySelect(ITractorableEntity entity);
        void TryRelease();
    }
}
