using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorableEntity : FarseerEntity, ITractorableEntity
    {
        public ITractorBeam TractorBeam { get; set; }

        public event EventHandler<ITractorableEntity> OnSelected;
        public event EventHandler<ITractorableEntity> OnReleased;

        public TractorableEntity(EntityInfo info, IGame game) : base(info, game)
        {
        }



        public virtual Boolean CanBeSelectedBy(ITractorBeam tractorBeam)
        {
            return this.TractorBeam == null;
        }

        public void Released()
        {
            this.TractorBeam = null;

            this.OnReleased?.Invoke(this, this);
        }

        public void SelectedBy(ITractorBeam tractorBeam)
        {
            this.TractorBeam?.TryRelease();

            this.TractorBeam = tractorBeam;

            this.OnSelected?.Invoke(this, this);
        }
    }
}
