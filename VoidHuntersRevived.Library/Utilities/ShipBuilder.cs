using Guppy.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Simple class used to recersively
    /// import or export a ShipPart chain.
    /// 
    /// When importing, brand new ShipPart instances
    /// will be configured & returned automatically.
    /// </summary>
    public class ShipBuilder
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public ShipBuilder(EntityCollection entities)
        {
            _entities = entities;
        }
        #endregion

        #region Import Methods
        public ShipPart Import(Stream input)
        {
            var reader = new BinaryReader(input);

            return this.Import(reader);
        }
        public ShipPart Import(BinaryReader input)
        {
            var output = _entities.Create<ShipPart>(input.ReadString(), sp =>
            {
                sp.SetId(Guid.NewGuid());
            });


            foreach(ConnectionNode female in output.FemaleConnectionNodes)
            { // Iterate through each female node in the output...
                if(input.ReadBoolean())
                { // If the stream has this node marked as attached...
                    // Create a new child and attatch it to the current node.
                    this.Import(input).MaleConnectionNode.Attach(female);
                }
            }


            // Return the newly created ShipPart.
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
        public void Export(ShipPart input, BinaryWriter output)
        {
            output.Write(input.Handle);

            foreach(ConnectionNode female in input.FemaleConnectionNodes)
            { // Iterate through all females in the input...
                if(female.Attached)
                { // If the female is attached...
                    output.Write(true);

                    this.Export(
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
