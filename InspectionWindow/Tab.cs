using Microsoft.Xna.Framework;
using NoteKeeper.Shell.ContextMenus;
using NoteKeeper.Shell.ToolBar;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.IO;

#nullable enable

namespace NoteKeeper.InspectionWindow
{
    internal abstract class Tab
    {
        private string _sourcePath;

        public string SourcePath => _sourcePath;
        public string Name { get; set; }

        public event Action<ShellModes> ShellModeChangeRequested;

        public abstract ITabSerializer TabSerializer { get; }
        public abstract ITabViewport TabViewport { get; }
        public abstract TabHistory TabHistory { get; }
        public abstract PopWindow Window { get; }

        public abstract IToolMenu? ToolMenu { get; }

        public abstract ContextMenu? GetContextMenu(Point mousePosition);
        public abstract void ShellModeUpdated(ShellModes mode);
        public abstract void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e);

        public Tab(string sourcePath)
        {
            _sourcePath = sourcePath;
            Name = Path.GetFileName(sourcePath);
        }

        public void MoveSourcePath(string newPath)
        {
            File.Move(_sourcePath, newPath);
            _sourcePath = newPath;
        }

        public void RequestShellModeChange(ShellModes mode)
        {
            ShellModeChangeRequested?.Invoke(mode);
        }
    }
}
