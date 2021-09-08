using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Interfaces.Spells
{
    public interface ISpell : IEntity
    {
        Object Cast(ISpellCaster caster, Object args);
    }
}
