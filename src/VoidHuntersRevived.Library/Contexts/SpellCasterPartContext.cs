using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    public abstract class SpellCasterPartContext : ShipPartContext
    {
        #region Enums
        private enum SpellCasterPartContextContext
        {
            Start = 0,
            End = 1,
            SpellCooldown = 2,
            SpellEnergyCost = 3
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The spell caster's cooldown (in seconds).
        /// </summary>
        [ShipPartContextProperty("Spell Cooldown", "The spell caster's cooldown (in seconds).", ShipPartContextPropertyType.Single)]

        public Single SpellCooldown { get; set; }
        /// <summary>
        /// The energy cost required for this spellcaster to function.
        /// </summary>
        [ShipPartContextProperty("Spell Energy Cost", "The energy cost required for this spellcaster to function.", ShipPartContextPropertyType.Single)]

        public Single SpellEnergyCost { get; set; }
        #endregion

        #region Constructors
        protected SpellCasterPartContext(string name) : base(name)
        {
        }
        #endregion

        #region Serialization Methods
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            SpellCasterPartContextContext propertyType = (SpellCasterPartContextContext)reader.ReadByte();
            if (propertyType == SpellCasterPartContextContext.Start)
            {
                while (propertyType != SpellCasterPartContextContext.End)
                {
                    propertyType = (SpellCasterPartContextContext)reader.ReadByte();
                    switch (propertyType)
                    {
                        case SpellCasterPartContextContext.SpellCooldown:
                            this.SpellCooldown = reader.ReadSingle();
                            break;
                        case SpellCasterPartContextContext.SpellEnergyCost:
                            this.SpellEnergyCost = reader.ReadSingle();
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((Byte)SpellCasterPartContextContext.Start);

            writer.Write((Byte)SpellCasterPartContextContext.SpellCooldown);
            writer.Write(this.SpellCooldown);

            writer.Write((Byte)SpellCasterPartContextContext.SpellEnergyCost);
            writer.Write(this.SpellEnergyCost);

            writer.Write((Byte)SpellCasterPartContextContext.End);
        }
        #endregion
    }
}
