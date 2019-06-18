using Guppy.Network.Security;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
using Guppy.UI.Enums;
using Guppy.UI.Styles;
using Guppy.UI.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.UI
{
    public class ChatItem : FancyTextElement
    {
        public readonly String Message;
        public readonly User User;

        public ChatItem(String message, UnitRectangle outerBounds, Element parent, Stage stage, Style style = null, User user = null) : base(outerBounds, parent, stage, style)
        {
            this.Message = message;
            this.User = user;

            if (this.User != null)
                this.Add($"{this.User.Get("name")}: ", Color.Red);

            this.Add($"{this.Message}", Color.White);
        }

        public override void Update(MouseState mouse)
        {
            base.Update(mouse);
        }
    }
}
