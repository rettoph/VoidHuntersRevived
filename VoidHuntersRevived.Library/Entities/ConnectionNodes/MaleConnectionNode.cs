using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class MaleConnectionNode : ConnectionNode
    {
        private static Vector3[] DebugVertices;

        public MaleConnectionNode(ShipPart parent, float rotation, Vector2 position, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(parent, rotation, position, configuration, scene, provider, logger)
        {
        }
        static MaleConnectionNode()
        {
            MaleConnectionNode.DebugVertices = new Vector3[] {
                new Vector3(-0.3f, -0.1f, 0),
                new Vector3(0.3f, -0.1f, 0),

                new Vector3(0.3f, -0.1f, 0),
                new Vector3(0.3f, 0.1f, 0),

                new Vector3(0.3f, 0.1f, 0),
                new Vector3(-0.3f, 0.1f, 0),

                new Vector3(-0.3f, 0.1f, 0),
                new Vector3(-0.3f, -0.1f, 0),
            };
        }

        public override void AddDebugVertices(ref List<VertexPositionColor> vertices)
        {
            var matrix = Matrix.CreateRotationZ(this.Rotation) * Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0) * Matrix.CreateRotationZ(this.parent.Rotation) * Matrix.CreateTranslation(this.parent.Position.X, this.parent.Position.Y, 0);

            foreach (Vector3 vertice in MaleConnectionNode.DebugVertices)
            {
                vertices.Add(new VertexPositionColor(Vector3.Transform(vertice, matrix), Color.Green));
            }
        }
    }
}
