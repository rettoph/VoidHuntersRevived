using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.System;
using VoidHuntersRevived.Library.Scenes;
using Guppy.IO.Extensions.log4net;

namespace VoidHuntersRevived.Server.Scenes
{
    public sealed class ServerGameScene : GameScene
    {
        #region Lifecycle Methods
        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);


            this.log.Debug(() => "Server");

            this.Entities.Create<WorldEntity>((w, p, c) =>
            {
                w.Size = new Vector2(Chunk.Size * 10, Chunk.Size * 10);
            });

            var rand = new Random(1);
            for(Int32 i=0; i<1; i++)
            {
                var triangle = this.Entities.Create<ShipPart>("entity:ship-part:hull:triangle");
                triangle.Position = rand.NextVector2(0, Chunk.Size * 10);
                triangle.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                var square = this.Entities.Create<ShipPart>("entity:ship-part:hull:square");
                square.Position = rand.NextVector2(0, Chunk.Size * 10);
                square.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                var hexagon = this.Entities.Create<ShipPart>("entity:ship-part:hull:hexagon");
                hexagon.Position = rand.NextVector2(0, Chunk.Size * 10);
                hexagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                var pentagon = this.Entities.Create<ShipPart>("entity:ship-part:hull:pentagon");
                pentagon.Position = rand.NextVector2(0, Chunk.Size * 10);
                pentagon.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            
                if (i % 2 == 0)
                {
                    var vBeam = this.Entities.Create<ShipPart>("entity:ship-part:hull:beam:vertical");
                    vBeam.Position = rand.NextVector2(0, Chunk.Size * 10);
                    vBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                }
                else
                {
                    var hBeam = this.Entities.Create<ShipPart>("entity:ship-part:hull:beam:horizontal");
                    hBeam.Position = rand.NextVector2(0, Chunk.Size * 10);
                    hBeam.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                }
            
                for (Int32 j = 0; j < 5; j++)
                {
                    var thruster = this.Entities.Create<ShipPart>("entity:ship-part:thruster:small");
                    thruster.Position = rand.NextVector2(0, Chunk.Size * 10);
                    thruster.Rotation = rand.NextSingle(-MathHelper.Pi, MathHelper.Pi);
                }
            }
        }
        #endregion
    }
}
