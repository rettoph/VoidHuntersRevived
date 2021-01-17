using Guppy;
using Guppy.IO.Services;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.Contexts;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// Simple service specifically designed for 
    /// editing a pre-existing shape context.
    /// 
    /// Note, some of the original transformation 
    /// data will  be lost when importing 
    /// pre-existing shape files. This is unavoidable.
    /// </summary>
    public class ShipPartShapeEditService : ShipPartShapesServiceChildBase
    {
        #region Private Fields
        private ShapeContext _shape;
        private Boolean _editing;
        #endregion

        #region API Methods
        public void Start(ShapeContext shape)
        {
            _shape = shape;
            _editing = true;
        }
        #endregion
    }
}
