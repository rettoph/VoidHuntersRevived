using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using System.Linq;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    // internal sealed class BodyEntityFullAuthorizationNetworkDriver : NetworkEntityNetworkDriver<BodyEntity>
    // {
    //     #region Private Fields
    //     private ActionTimer _timer;
    //     #endregion
    // 
    //     #region Lifecycle Methods
    //     protected override void ConfigureFull(ServiceProvider provider)
    //     {
    //         base.ConfigureFull(provider);
    // 
    //         _timer = new ActionTimer(150);
    // 
    //         this.driven.OnUpdate += this.Update;
    //         this.driven.OnWrite += this.WritePosition;
    // 
    //         this.driven.Actions.Set("update:position", this.ReadPosition);
    //     }
    // 
    //     protected override void DisposeFull()
    //     {
    //         base.DisposeFull();
    // 
    //         this.driven.OnUpdate -= this.Update;
    //         this.driven.OnWrite -= this.WritePosition;
    // 
    //         this.driven.Actions.Remove("update:position");
    //     }
    //     #endregion
    // 
    //     #region Frame Methods
    //     private void Update(GameTime gameTime)
    //     {
    //         _timer.Update(gameTime, t => t && this.driven.Fixtures.Any(), () =>
    //         { // Broadcast a positional upddate
    //             this.WritePosition(this.driven.Actions.Create(NetDeliveryMethod.UnreliableSequenced, 8));
    //         });
    //     }
    //     #endregion
    // 
    //     #region Network Methods
    //     private void WritePosition(NetOutgoingMessage om)
    //         => BodyEntityFullAuthorizationNetworkDriver.WritePosition(this.driven, om);
    // 
    //     private void ReadPosition(NetIncomingMessage im)
    //     {
    //         // Nothing should happen when info is read on full control...
    //         im.ReadVector2();
    //         im.ReadSingle();
    // 
    //         im.ReadVector2();
    //         im.ReadSingle();
    //     }
    //     #endregion
    // 
    //     #region Static Methods
    //     public static void WritePosition(BodyEntity entity, NetOutgoingMessage om)
    //     {
    //         om.Write("update:position", m =>
    //         {
    //             m.Write(entity.Position);
    //             m.Write(entity.Rotation);
    // 
    //             m.Write(entity.LinearVelocity);
    //             m.Write(entity.AngularVelocity);
    //         });
    //     }
    //     #endregion
    // }
}
