using Microsoft.Xna.Framework;
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
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } = "Unnamed";

        public abstract ITabViewport TabViewport { get; }
        public abstract PopWindow Window { get; }

        public abstract void ShellModeUpdated(ShellModes mode);
        public abstract void AddContent(IPageContent content);
        public abstract Point GetContentScreenLocation(IPageContent content);

        public abstract void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e);
    }
}
