using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public void Import(Byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
                this.Import(stream);
        }
        public void Import(Stream input)
        {
            var reader = new BinaryReader(input);

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
                        this.SetBridge(this.ImportComponents(reader));
                        break;
                }
            }
        }
        private ShipPart ImportComponents(BinaryReader input)
        {
            var output = _entities.Create<ShipPart>(input.ReadString(), (sp, p, s) =>
            {
                sp.Id = Guid.NewGuid();
            });

            foreach (ConnectionNode female in output.FemaleConnectionNodes)
            { // Iterate through each female node in the output...
                if (input.ReadBoolean())
                { // If the stream has this node marked as attached...
                    // Create a new child and attatch it to the current node.
                    this.ImportComponents(input).MaleConnectionNode.TryAttach(female);
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
            output.Write(input.ServiceDescriptor.Name);

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
