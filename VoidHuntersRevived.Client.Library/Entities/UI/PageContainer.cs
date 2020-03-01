using Guppy.UI.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Entities.UI
{
    /// <summary>
    /// Simple element designed to contain multiple pages.
    /// Pages my be toggled, or even closed entirely when opened.
    /// </summary>
    public class PageContainer : Container<Page>
    {
        private Page _current;
        private Page _target;
        private Boolean _changing;

        /// <summary>
        /// Open the recieved page.
        /// </summary>
        /// <param name="page"></param>
        public void Open(Page page)
        {
            if (this.children.Contains(page))
                throw new Exception("Unable to open unrelated page. Please ensure the given page is a child of the current container.");

            _target = page;
        }

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}
