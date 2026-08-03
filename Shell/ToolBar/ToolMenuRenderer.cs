using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuRenderer
    {
        private readonly ToolMenuStyleProvider _styleProvider;
        private readonly ToolMenuLayoutCalculator _layoutCalculator;

        public ToolMenuRenderer(ToolMenuStyleProvider styleProvider, ToolMenuLayoutCalculator layoutCalculator)
        {
            _styleProvider = styleProvider;
            _layoutCalculator = layoutCalculator;
        }
    }
}
