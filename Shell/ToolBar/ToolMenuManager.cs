using PopUpUI.Elements;
using PopUpUI.Elements.Visual;
using PopUpUI.Input;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolsMenuManager
    {
        private readonly ToolMenuRenderer renderer;
        private readonly ToolMenuLayoutCalculator layoutCalculator;

        private IToolMenu? _shownMenu;

        public ToolMenuStyleProvider StyleProvider { get; }

        public ToolsMenuManager()
        {
            StyleProvider = new ToolMenuStyleProvider();

            layoutCalculator = new ToolMenuLayoutCalculator(StyleProvider);
            renderer = new ToolMenuRenderer(StyleProvider, layoutCalculator);
        }

        public void Show(IToolMenu menu)
        {
        }

        public void Hide()
        {

        }

        public void MousePositionChangeUpdate(PopWindowMouseEventArgs e)
        {

        }

        public void MouseButtonStateChange(PopWindowMouseEventArgs e)
        {

        }
    }
}
