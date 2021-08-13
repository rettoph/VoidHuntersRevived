using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Guppy.Utilities.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Services
{
    public class ShipPartRenderService : Frameable
    {
        #region Private Structs
        private struct ShipPartContextPrimitiveData
        {
            public PrimitiveShape[] Shapes;
        }
        #endregion

        #region Private Fields
        private Dictionary<UInt32, ShipPartContextPrimitiveData> _primitives;

        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private ShipPartService _shipParts;

        private PrimitivePath _nodePrimitive;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            _primitives = new Dictionary<UInt32, ShipPartContextPrimitiveData>();

            provider.Service(out _primitiveBatch);
            provider.Service(out _shipParts);

            _nodePrimitive = PrimitivePath.Create(0.025f, new Vector2(0.2f, -0.1f), new Vector2(0.025f, 0), new Vector2(0.2f, 0.1f));
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            foreach (ShipPartContext context in _shipParts.RegisteredContexts.Values)
                this.RegisterContextPrimitiveData(context);

            _shipParts.OnContextRegistered += this.HandleContextRegistered;
        }

        protected override void Release()
        {
            base.Release();

            _shipParts.OnContextRegistered -= this.HandleContextRegistered;
        }

        protected override void PostRelease()
        {
            base.Release();

            _primitives.Clear();

            _primitiveBatch = null;
            _shipParts = null;
        }
        #endregion

        #region Frame Methods
        /// <summary>
        /// Render the specific ShipPart recieved, if possible.
        /// </summary>
        /// <param name="shipPart"></param>
        public void Render(ShipPart shipPart, ref Matrix worldTransformation)
        {
            foreach(PrimitiveShape shape in _primitives[shipPart.Context.Id].Shapes)
            { // Draw all shapes...
                _primitiveBatch.DrawPrimitive(
                    shape,
                    Color.Green,
                    worldTransformation);

                // _primitiveBatch.TryFlushTriangleVertices(true);

                foreach(ConnectionNode node in shipPart.ConnectionNodes)
                {
                    Color color = default;

                    switch (node.Connection.State)
                    {
                        case ConnectionNodeState.Estranged:
                            color = Color.Red;
                            break;
                        case ConnectionNodeState.Parent:
                            color = Color.Blue;
                            break;
                        case ConnectionNodeState.Child:
                            color = Color.White;
                            break;
                    }

                    var nodeWorldTransformation = node.LocalTransformationMatrix * worldTransformation;

                    _primitiveBatch.DrawPrimitive(
                        _nodePrimitive,
                        color,
                        nodeWorldTransformation);

                    // _primitiveBatch.TryFlushLineVertices(true);
                }
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Construct a new <see cref="ShipPartContextPrimitiveData"/> instance based on
        /// ths given <paramref name="context"/> value.
        /// </summary>
        /// <param name="context"></param>
        private void RegisterContextPrimitiveData(ShipPartContext context)
        {
            _primitives[context.Id] = new ShipPartContextPrimitiveData()
            {
                Shapes = context.Shapes.Select(s => PrimitiveShape.Create(s.Vertices)).ToArray()
            };
        }
        #endregion

        #region Event Handlers
        private void HandleContextRegistered(ShipPartService sender, ShipPartContext args)
            => this.RegisterContextPrimitiveData(args);
        #endregion
    }
}
