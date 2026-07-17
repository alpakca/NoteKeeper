using PopUpUI.Surface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using PopUpUI.Input;

namespace NoteKeeper.InspectionWindow.PagedTabs.SinglePage
{
    // talks to shell - only class that have connection between shell and inspection items
    internal class SinglePageTab : PageContentTab
    {
        private readonly NotePage page;
        private readonly TabViewport viewport;
        private readonly TabWindow window;

        public override ITabViewport TabViewport => viewport;
        public override TabWindow Window => window;

        public SinglePageTab(NotePage page, Point windowSize)
        {
            viewport = new TabViewport { UISize = windowSize };

            this.page = page;
            window = new TabWindow(page, viewport);
        }

        public override void ShellModeUpdated(ShellModes mode)
        {
            window.ModeUpdated(mode);
        }

        public override void AddContent(IPageContent content)
        {
            page.Contents.Add(content);
        }

        public override Point GetContentScreenLocation(IPageContent content)
        {
            return viewport.ToScreenLocation(content.Location);
        }

        public override void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e)
        {
            window.SimulateMouseButtonStateInput(e);
        }
    }
}
