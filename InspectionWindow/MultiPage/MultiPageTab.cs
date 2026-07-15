using Microsoft.Xna.Framework;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class MultiPageTab : Tab
    {
        private bool inVerticalMode = true;
        private readonly VerticalModeViewport verticalModeViewport;
        private readonly HorizontalModeViewport horizontalModeViewport;

        public override ITabViewport TabViewport => inVerticalMode ? verticalModeViewport : horizontalModeViewport;
        public override PopWindow Window => throw new NotImplementedException();

        public MultiPageTab()
        {
            verticalModeViewport = new VerticalModeViewport();
            horizontalModeViewport = new HorizontalModeViewport();
        }

        public override void AddContent(IPageContent content)
        {
            throw new NotImplementedException();
        }

        public override Point GetContentScreenLocation(IPageContent content)
        {
            throw new NotImplementedException();
        }

        public override void ShellModeUpdated(ShellModes mode)
        {
            throw new NotImplementedException();
        }

        public override void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
