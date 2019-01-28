using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Client.Entities.Ships
{
    class CurrentUserShip : Ship
    {
        private ClientMainScene _scene;

        public CurrentUserShip(EntityInfo info, IGame game) : base(info, game)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as ClientMainScene;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.TractorBeam.Body.Position = _scene.Cursor.Body.Position;

            if(Mouse.GetState().RightButton == ButtonState.Pressed)
                if(_scene.Cursor.Over is ITractorableEntity)
                    this.TractorBeam.TrySelect(_scene.Cursor.Over as ITractorableEntity);

            if (Mouse.GetState().RightButton == ButtonState.Released)
                if (this.TractorBeam.SelectedEntity != null)
                    this.TractorBeam.TryRelease();
        }
    }
}
