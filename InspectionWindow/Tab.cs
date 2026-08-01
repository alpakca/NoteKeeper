using Microsoft.Xna.Framework;
using NoteKeeper.Shell.ContextMenus;
using PopUpUI.Input;
using PopUpUI.Surface;

#nullable enable

namespace NoteKeeper.InspectionWindow
{
    internal abstract class Tab
    {
        public string Name { get; set; } = "Unnamed";

        public abstract ITabSerializer TabSerializer { get; }
        public abstract ITabViewport TabViewport { get; }
        public abstract TabHistory TabHistory { get; }
        public abstract PopWindow Window { get; }

        public abstract ContextMenu? GetContextMenu(Point mousePosition);
        public abstract void ShellModeUpdated(ShellModes mode);
        public abstract void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e);
    }
}
