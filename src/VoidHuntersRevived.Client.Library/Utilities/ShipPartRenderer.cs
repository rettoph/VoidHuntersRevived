using FarseerPhysics.Common;
using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    /// <summary>
    /// Simple helper service to render a ship part.
    /// </summary>
    internal sealed class ShipPartRenderer : Service
    {
        #region Private Fields
        private PrimitiveBatch _primitiveBatch;
        private Vector2[] _tempVertices;
        private Color _fill;
        private Color _border;
        private Vector2 _nodeVertice1;
        private Vector2 _nodeVertice2;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _primitiveBatch);

            // Create temp buffer for hulls.
            _tempVertices = new Vector2[20];
            _nodeVertice1 = new Vector2(-0.15f, 0.09f);
            _nodeVertice2 = new Vector2(-0.15f, -0.09f);
        }
        #endregion

        #region Frame Methods
        public void Draw(ShipPart shipPart)
        {
            _fill = Color.Lerp(shipPart.Color, Color.TransparentBlack, 0.25f);
            _border = Color.Lerp(shipPart.Color, Color.White, 0.25f);

            int vertexCount = shipPart.Configuration.Hull.Count;
            var worldMatrix = Matrix.CreateRotationZ(shipPart.Root.Rotation) * Matrix.CreateTranslation(shipPart.Root.Position.X, shipPart.Root.Position.Y, 0);
            var partMatrix = shipPart.LocalTransformation * worldMatrix;

            for(Int32 i=0; i< vertexCount; i++)
                _tempVertices[i] = Vector2.Transform(shipPart.Configuration.Hull[i], partMatrix);

            for (int i = 1; i < vertexCount - 1; i++)
            {
                _primitiveBatch.DrawTriangle(_tempVertices[0], _fill, _tempVertices[i], _fill, _tempVertices[i + 1], _fill);
                _primitiveBatch.DrawLine(_tempVertices[i], _tempVertices[i - 1], _border);
            }

            _primitiveBatch.DrawLine(_tempVertices[0], _tempVertices[vertexCount - 1], _border);
            _primitiveBatch.DrawLine(_tempVertices[vertexCount - 1], _tempVertices[vertexCount - 2], _border);

            // Draw the male connection node...
            var nodeMatrix = shipPart.MaleConnectionNode.LocalTransformationMatrix * partMatrix;
            _tempVertices[0] = Vector2.Transform(Vector2.Zero, nodeMatrix);
            _tempVertices[1] = Vector2.Transform(_nodeVertice1, nodeMatrix);
            _tempVertices[2] = Vector2.Transform(_nodeVertice2, nodeMatrix);
            _primitiveBatch.DrawLine(_tempVertices[0], _tempVertices[1], _border);
            _primitiveBatch.DrawLine(_tempVertices[0], _tempVertices[2], _border);
        }
        #endregion
    }
}
