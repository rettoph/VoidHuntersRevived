using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Dtos.Utilities;
using Guppy.Network.Interfaces;
using VoidHuntersRevived.Library.Utilities;
using LiteNetLib.Utils;
using Guppy.Network;
using Guppy.Threading.Interfaces;

namespace VoidHuntersRevived.Library.Messages.Network.Packets
{
    public sealed class ShipPartPacket : IData
    {
        /// <summary>
        /// <para>The <see cref="ShipPart"/>'s <see cref="ShipPart.NetworkId"/></para>
        /// <para>This should always be defined</para>
        /// </summary>
        public UInt16 NetworkId { get; init; }

        /// <summary>
        /// Indicates what data is contained within the current message. 
        /// </summary>
        public ShipPartSerializationFlags SerializationFlags { get; init; }

        /// <summary>
        /// <para>The <see cref="ShipPart"/>'s <see cref="ShipPartContext.Id"/></para>
        /// <para>Only defined if <see cref="SerializationFlags"/> has <see cref="ShipPartSerializationFlags.Create"/></para>
        /// </summary>
        public UInt32? ContextId { get; private set; }

        /// <summary>
        /// <para>The <see cref="ShipPart"/>'s Immidiate children</para>
        /// <para>Only defined if <see cref="SerializationFlags"/> has <see cref="ShipPartSerializationFlags.Tree"/></para>
        /// </summary>
        public IEnumerable<ConnectionNodeDto> Children { get; private set; }

        #region Constructors
        private ShipPartPacket()
        {

        }
        public ShipPartPacket(ShipPart source, ShipPartSerializationFlags serializationFlags)
        {
            this.NetworkId = source.NetworkId;
            this.SerializationFlags = serializationFlags;

            if((serializationFlags & ShipPartSerializationFlags.Create) != 0)
            {
                this.ContextId = source.Context.Id;
            }

            if ((serializationFlags & ShipPartSerializationFlags.Tree) != 0)
            {
                List<ConnectionNodeDto> children = new List<ConnectionNodeDto>();
                foreach(ConnectionNode node in source.ConnectionNodes)
                {
                    if (node.Connection.State == ConnectionNodeState.Parent)
                    {
                        children.Add(new ConnectionNodeDto()
                        {
                            Index = node.Index,
                            State = ConnectionNodeState.Parent,
                            TargetNodeIndex = node.Connection.Target.Index,
                            TargetNodeOwner = new ShipPartPacket(node.Connection.Target.Owner, serializationFlags)
                        });
                    }
                    else if (node.Connection.State == ConnectionNodeState.Estranged)
                    {
                        children.Add(new ConnectionNodeDto()
                        {
                            Index = node.Index,
                            State = ConnectionNodeState.Estranged
                        });
                    }
                }

                this.Children = children;
            }
            else
            {
                this.Children = Enumerable.Empty<ConnectionNodeDto>();
            }
        }
        #endregion

        internal static ShipPartPacket Read(NetDataReader reader, NetworkProvider network)
        {
            ShipPartPacket packet = new ShipPartPacket()
            {
                NetworkId = reader.GetUShort(),
                SerializationFlags = reader.GetEnum<ShipPartSerializationFlags>()
            };

            if ((packet.SerializationFlags & ShipPartSerializationFlags.Create) != 0)
            {
                packet.ContextId = reader.GetUInt();
            }

            if ((packet.SerializationFlags & ShipPartSerializationFlags.Tree) != 0)
            {
                ConnectionNodeState state;
                List<ConnectionNodeDto> children = new List<ConnectionNodeDto>();

                while((state = reader.GetEnum<ConnectionNodeState>()) != ConnectionNodeState.Child)
                {
                    ConnectionNodeDto child = new ConnectionNodeDto()
                    {
                        Index = reader.GetByte(),
                        State = state
                    };

                    if (child.State == ConnectionNodeState.Parent)
                    {
                        child.TargetNodeIndex = reader.GetByte();

                        child.TargetNodeOwner = ShipPartPacket.Read(reader, network);
                    }

                    children.Add(child);
                }

                packet.Children = children;
            }

            return packet;
        }

        internal static void Write(NetDataWriter writer, NetworkProvider network, ShipPartPacket packet)
        {
            writer.Put(packet.NetworkId);
            writer.Put(packet.SerializationFlags);
            
            if((packet.SerializationFlags & ShipPartSerializationFlags.Create) != 0)
            {
                writer.Put(packet.ContextId.Value);
            }

            if ((packet.SerializationFlags & ShipPartSerializationFlags.Tree) != 0)
            {
                foreach(ConnectionNodeDto child in packet.Children)
                {
                    writer.Put(child.State);
                    writer.Put(child.Index);

                    if(child.State == ConnectionNodeState.Parent)
                    {
                        writer.Put(child.TargetNodeIndex.Value);

                        ShipPartPacket.Write(writer, network, child.TargetNodeOwner);
                    }
                }

                // For the purposes of the reader, a child byte marks the end of the tree.
                writer.Put(ConnectionNodeState.Child);
            }
        }
    }
}
