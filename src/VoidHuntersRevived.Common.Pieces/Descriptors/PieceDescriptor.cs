﻿using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public abstract class PieceDescriptor : VoidHuntersEntityDescriptor
    {
        public PieceDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Node>(
                    builder: new ComponentBuilder<Node>(),
                    serializer: new ComponentSerializer<Node>(
                        writer: (entities, writer, node) =>
                        {
                            writer.Write(node.TreeId.VhId);
                        },
                        reader: (entities, reader) =>
                        {
                            // If no seed is passed the tree should be read, if a seed is passed we assume it is the id of the owning tree
                            // This is relevant during Node deletion revision and Tree creation from cloned data within TreeFactory
                            VhId treeVhId = reader.ReadVhId();
                            treeVhId = reader.Seed.Value == VhId.Empty.Value ? treeVhId : reader.Seed;
                            return  new Node(entities.GetId(treeVhId));
                        })),
                new ComponentManager<Rigid>(
                    builder: new ComponentBuilder<Rigid>(),
                    serializer: new ComponentSerializer<Rigid>(
                        writer: (entites, writer, instance) =>
                        {
                            writer.WriteNativeDynamicArray(instance.Shapes, PolygonWriter);
                        },
                        reader: (entites, reader) => new Rigid()
                        {
                            Shapes = reader.ReadNativeDynamicArray<Polygon>(PolygonReader)
                        })),
                new ComponentManager<Visible>(
                    builder: new ComponentBuilder<Visible>(),
                    serializer: new ComponentSerializer<Visible>(
                        writer: (entites, writer, instance) =>
                        {
                            writer.WriteNativeDynamicArray(instance.Shapes, ShapeWriter);
                            writer.WriteNativeDynamicArray(instance.Paths, ShapeWriter);
                        },
                        reader: (entites, reader) => new Visible()
                        {
                            Shapes = reader.ReadNativeDynamicArray<Shape>(ShapeReader),
                            Paths = reader.ReadNativeDynamicArray<Shape>(ShapeReader)
                        })),
            });
        }

        private Shape ShapeReader(EntityReader reader)
        {
            return new Shape()
            {
                Color = reader.ReadStruct<EntityResource<Color>>(),
                Vertices = reader.ReadNativeDynamicArray<Vector3>()
            };
        }

        private void ShapeWriter(EntityWriter writer, Shape shape)
        {
            writer.WriteStruct(shape.Color);
            writer.WriteNativeDynamicArray(shape.Vertices);
        }

        private Polygon PolygonReader(EntityReader reader)
        {
            return new Polygon()
            {
                Density = reader.ReadStruct<Fix64>(),
                Vertices = reader.ReadNativeDynamicArray<FixVector2>()
            };
        }

        private void PolygonWriter(EntityWriter writer, Polygon polygon)
        {
            writer.WriteStruct(polygon.Density);
            writer.WriteNativeDynamicArray(polygon.Vertices);
        }
    }
}
