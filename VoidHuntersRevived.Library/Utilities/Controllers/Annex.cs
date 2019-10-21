using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Utilities.Controllers
{
    /// <summary>
    /// Empty controller that contains no functionality.
    /// 
    /// This is where pieces go when they spawn and have
    /// not yet been assigned a controller.
    /// </summary>
    public class Annex : Controller<FarseerEntity>
    {
        protected internal override void Remove(object entity)
        {
            // throw new NotImplementedException();
        }
    }
}
