using FarseerPhysics.Common;
using Guppy.Loaders;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities.ConnectionNodes;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Simple static class used to auto generate 
    /// ship part data on initialize time.
    /// An entity loader, description, name,
    /// and side count must be inserted to use.
    /// </summary>
    public static class ShipPartRegistrar
    {
        public static void RegisterPolygon<TShipPart>(EntityLoader entitityLoader, String handle, String nameHandle, String descriptionHandle, Int32 sides, Boolean includeFemales = false)
            where TShipPart : ShipPart
        {
            if (sides < 3)
                throw new Exception("Unable to create polygon with less than three sides!");

            // Side lengths are always 1. The male connection node is inserted in the middle of the first side
            // and the female connection nodes are inserted on the following sides (if requested)
            Vector2[] vertices = new Vector2[sides];
            Vector3[] femaleNodes = new Vector3[sides - 1];
            Vector3 maleNode = default(Vector3);

            Single angle = 0;
            Single targetAngle = (sides == 3 ? MathHelper.TwoPi : MathHelper.TwoPi);
            Single stepAngle = targetAngle / sides;
            Vector2 vertice = Vector2.Zero;
            Matrix rotation;

            for (Int32 i=0; i<sides; i++)
            {
                vertices[i] = new Vector2(vertice.X, vertice.Y);

                angle += stepAngle;
                rotation = Matrix.CreateRotationZ(angle);

                if (i == 0)
                { // Set the male node data
                    maleNode = new Vector3(vertice + Vector2.Transform(new Vector2(0.5f, 0), rotation), angle - MathHelper.PiOver2);
                }
                else
                { // Set the female node data
                    femaleNodes[i-1] = new Vector3(vertice + Vector2.Transform(new Vector2(0.5f, 0), rotation), angle + MathHelper.PiOver2);
                }

                vertice += Vector2.Transform(new Vector2(1f, 0), rotation);
            }

            // Once everything is generated, register the entity
            entitityLoader.Register<TShipPart>(handle, nameHandle, descriptionHandle, new ShipPartConfiguration(new Vertices(vertices), maleNode, femaleNodes));
        }
    }
}
