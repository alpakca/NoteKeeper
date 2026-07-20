using Microsoft.Xna.Framework;
using NoteKeeper.Shell.ContextMenus;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteKeeper.InspectionWindow
{
    internal abstract class Tab
    {
        public string Name { get; set; } = "Unnamed";

        public abstract ITabViewport TabViewport { get; }
        public abstract PopWindow Window { get; }

        public abstract ContextMenu? GetContextMenu(Point mousePosition);
        public abstract void ShellModeUpdated(ShellModes mode);
        public abstract void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e);
    }
}
