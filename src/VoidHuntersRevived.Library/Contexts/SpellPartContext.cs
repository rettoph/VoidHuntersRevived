using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    public abstract class SpellPartContext : ShipPartContext
    {
        #region Enums
        private enum SpellCasterPartContextContext
        {
            Start = 0,
            End = 1,
            SpellCooldown = 2,
            SpellManaCost = 3,
            IndicatorRadius = 4
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
        [ShipPartContextProperty("Spell Mana Cost", "The mana cost required for this spellcaster to function.", ShipPartContextPropertyType.Single)]

        public Single SpellManaCost { get; set; }

        /// <summary>
        /// The energy cost required for this spellcaster to function.
        /// </summary>
        [ShipPartContextProperty("Indicator Radius", "The radius of the client side cooldown indicator.", ShipPartContextPropertyType.Single)]

        public Single IndicatorRadius { get; set; } = 0.15f;
        #endregion

        #region Constructors
        protected SpellPartContext(string name) : base(name)
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
                        case SpellCasterPartContextContext.SpellManaCost:
                            this.SpellManaCost = reader.ReadSingle();
                            break;
                        case SpellCasterPartContextContext.IndicatorRadius:
                            this.IndicatorRadius = reader.ReadSingle();
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

            writer.Write((Byte)SpellCasterPartContextContext.SpellManaCost);
            writer.Write(this.SpellManaCost);

            writer.Write((Byte)SpellCasterPartContextContext.IndicatorRadius);
            writer.Write(this.IndicatorRadius);

            writer.Write((Byte)SpellCasterPartContextContext.End);
        }
        #endregion
    }
}
