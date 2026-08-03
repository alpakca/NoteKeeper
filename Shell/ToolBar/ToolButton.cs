using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolButton
    {
        public string ID { get; }
        public Action ClickAction { get; }

        public ToolButton(string id, Action clickAction)
        {
            ID = id;
            ClickAction = clickAction;
        }
    }
}
