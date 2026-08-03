using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolButton
    {
        private readonly Action _clickAction;

        public string ID { get; }
        public bool Enabled { get; set; } = true;

        public ToolButton(string id, Action clickAction)
        {
            ID = id;
            _clickAction = clickAction;
        }

        public void Selected()
        {
            _clickAction.Invoke();
        }
    }
}
