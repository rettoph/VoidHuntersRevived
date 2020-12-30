using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    public partial class Ship
    {
        #region Enum
        private enum ExportData : Byte
        {
            Title,
            Components,
        }
        #endregion

        #region Import Methods
        public void Import(Byte[] data, Vector2 position = default, Single rotation = default)
        {
            using (MemoryStream stream = new MemoryStream(data))
                this.Import(stream, position);
        }
        public void Import(Stream input, Vector2 position = default, Single rotation = default)
        {
            using (BinaryReader reader = new BinaryReader(input))
            {
                while (input.Position < input.Length)
                {
                    switch ((Ship.ExportData)reader.ReadByte())
                    {
                        // Import ship title data...
                        case ExportData.Title:
                            this.Title = reader.ReadString();
                            break;
                        // Import ship component data...
                        case ExportData.Components:
                            this.Bridge = this.ImportComponents(reader, position, rotation);
                            break;
                    }
                }
            }
        }
        private ShipPart ImportComponents(BinaryReader input, Vector2 position = default, Single rotation = default)
        {
            var output = _entities.Create<ShipPart>(input.ReadString(), (sp, p, s) =>
            {
                sp.Id = Guid.NewGuid();
                sp.SetTransformIgnoreContacts(position, rotation);
            });

            foreach (ConnectionNode female in output.FemaleConnectionNodes)
            { // Iterate through each female node in the output...
                if (input.ReadBoolean())
                { // If the stream has this node marked as attached...
                    // Create a new child and attatch it to the current node.
                    this.ImportComponents(input, position, rotation).MaleConnectionNode.TryAttach(female);
                }
            }

            return output;
        }
        #endregion

        #region Export Methods
        public MemoryStream Export()
        {
            var output = new MemoryStream();
            var writer = new BinaryWriter(output);

            // Write the ship title...
            writer.Write((Byte)Ship.ExportData.Title);
            writer.Write(this.Title);

            // Write the ship component data...
            writer.Write((Byte)Ship.ExportData.Components);
            this.ExportComponents(this.Bridge, writer);

            return output;
        }
        private void ExportComponents(ShipPart input, BinaryWriter output)
        {
            output.Write(input.ServiceConfiguration.Name);

            foreach (ConnectionNode female in input.FemaleConnectionNodes)
            { // Iterate through all females in the input...
                if (female.Attached)
                { // If the female is attached...
                    output.Write(true);

                    this.ExportComponents(
                        input: female.Target.Parent,
                        output: output);
                }
                else
                { // If the female is not attached...
                    output.Write(false);
                }
            }
        }
        #endregion
    }
}
