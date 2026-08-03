using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuStyleProvider
    {
        private ToolMenuStyle _style;

        public ToolMenuStyle Style => _style;

        public void ApplyTheme(ToolMenuStyle style)
        {
            _style = style;
        }
    }
}
