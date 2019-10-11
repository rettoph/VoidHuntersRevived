using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Entities.ShipParts.ConnectionNodes;
using Guppy.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GalacticFighters.Library.Utilities
{
    /// <summary>
    /// Simple class used to recursively import or export
    /// a ship part chain. This should only be used to import
    /// via server side commands, but exporting can take place on
    /// the client.
    /// </summary>
    public class ShipBuilder
    {
        private EntityCollection _entities;

        public ShipBuilder(EntityCollection entities)
        {
            _entities = entities;
        }

        #region Import Methods
        public ShipPart Import(Stream input)
        {
            var reader = new BinaryReader(input);

            return this.Import(reader);
        }
        private ShipPart Import(BinaryReader input)
        {
            var output = _entities.Create<ShipPart>(input.ReadString(), sp =>
            {
                sp.SetId(Guid.NewGuid());
            });

            foreach (FemaleConnectionNode female in output.FemaleConnectionNodes)
            { // Iterate through each connection node...
                if (input.ReadBoolean())
                { // If there was any data attached to the specified node...
                    // Create a new child and attach to the current female...
                    this.Import(input).MaleConnectionNode.Attach(female);
                }
            }

            // Run the custom import functions
            output.Import(input);

            return output;
        }
        #endregion

        #region Export Methods
        public MemoryStream Export(ShipPart input)
        {
            var output = new MemoryStream();
            var writer = new BinaryWriter(output);

            this.Export(input, writer);

            return output;
        }
        private void Export(ShipPart input, BinaryWriter output)
        {
            output.Write(input.Configuration.Handle);

            foreach (FemaleConnectionNode female in input.FemaleConnectionNodes)
            { // Export all the child data...
                if (female.Attached)
                {
                    output.Write(true);
                    this.Export(
                        input: female.Target.Parent,
                        output: output);
                }
                else
                {
                    output.Write(false);
                }
            }

            // Run the custom export functions
            input.Export(output);
        }
        #endregion
    }
}
