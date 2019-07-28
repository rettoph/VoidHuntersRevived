using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Guppy.Network.Security;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
using Guppy.UI.Enums;
using Guppy.UI.Styles;
using Guppy.UI.Utilities;
using Guppy.UI.Utilities.Units.UnitValues;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.UI
{
    public class ChatWindow : Element
    {
        private Group _group;
        private DateTime _last;

        public Boolean Typing { get; private set; }
        public ScrollContainer Content { get; private set; }
        public TextInput Input { get; private set; }

        public ChatWindow(Group group, UnitRectangle outerBounds, Element parent, Stage stage, Style style = null) : base(outerBounds, parent, stage, style)
        {
            _group = group;

            this.Content = this.createElement<ScrollContainer>(0, 0, 1f, new UnitValue[] { 1f, -30 });
            this.Content.Style.Set<Color>(StateProperty.ScrollBarColor, Color.Transparent);
            this.Content.Style.Set<Color>(StateProperty.ScrollBarThumbColor, Color.DarkGray);
            this.Content.ScrollBar.Outer.Width.SetValue(3);
            this.Content.ScrollTo(1f);

            this.Input = this.createElement<TextInput>(0, new UnitValue[] { 1f, -30 }, 1f, 30);
            this.Input.MaxLength = 150;
            this.Input.Hidden = true;

            this.Input.OnEnter += this.HandleInputEnter;

            _group.Messages.AddHandler("chat", this.HandleChatMessage);
        }

        public override void Update(MouseState mouse)
        {
            base.Update(mouse);

            var kState = Keyboard.GetState();

            if(kState.IsKeyDown(Keys.T) && !this.Typing)
            {
                this.Typing = true;
                this.Input.Hidden = false;
                this.Content.Hidden = false;
                this.Input.Text = "";
                this.Input.Select();
                this.DirtyBounds = true;
            }
            else if(kState.IsKeyDown(Keys.Escape) && this.Typing)
            {
                this.Typing = false;
                this.Input.Hidden = true;
                this.Input.Deselect();
                _last = DateTime.Now;
            }

            if (DateTime.Now.Subtract(_last).TotalMilliseconds > 5000 && !this.Typing)
            {
                this.Content.Hidden = true;
                this.Content.ScrollTo(1f);
            }
        }

        private void HandleInputEnter(object sender, string e)
        {
            var om = _group.CreateMessage("chat", NetDeliveryMethod.ReliableOrdered, 2);
            om.Write(this.Input.Text);
            this.Input.Text = "";
            this.Typing = false;
            this.Input.Hidden = true;
            this.Input.Deselect();
        }

        private void HandleChatMessage(NetIncomingMessage obj)
        {
            if (obj.ReadBoolean())
            {
                var user = _group.Users.GetById(obj.ReadGuid());
                var message = obj.ReadString();

                this.Content.Items.CreateElement<ChatItem>(0, 0, 1f, 0, user, message);
            }
            else
            {
                var message = obj.ReadString();

                this.Content.Items.CreateElement<ChatItem>(0, 0, 1f, 0, message);
            }

            this.Content.Hidden = false;
            _last = DateTime.Now;
        }
    }
}
