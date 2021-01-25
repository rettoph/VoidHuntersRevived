using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using Guppy.Extensions.Utilities;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// Simple helper class useful for editing connection nodes.
    /// </summary>
    public class ConnectionNodeEditorService : ShipPartShapesServiceEditorChildBase<ConnectionNodeContext>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(this.flags.HasFlag(EditFlags.Editing))
                this.primitiveBatch.TraceCircle(
                    Color.Cyan,
                    this.camera.Position + this.item.Position,
                    0.1f);
        }

        protected override void Drag(ConnectionNodeContext item, Vector2 position)
        {
            item.Position = position;

            if (this.@lock[Enums.LockType.PointSnap])
            {
                var interestPoints = this.shapes.GetInterestPoints();

                foreach (Vector2 point in interestPoints)
                {
                    if (point != item.Position && Vector2.Distance(point, item.Position) < 0.25f)
                    {
                        item.Position = point;
                        break;
                    }
                }
            }
        }

        protected override Vector2 GetPosition(ConnectionNodeContext item)
            => item.Position;

        protected override bool ItemUnder(ConnectionNodeContext item, Vector2 worldPosition)
            => Vector2.Distance(item.Position, worldPosition) < 0.1f;

        protected override bool ShouldStop()
            => true;
        #endregion
    }
}
