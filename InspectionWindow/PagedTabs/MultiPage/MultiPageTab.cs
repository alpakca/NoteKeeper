using Microsoft.Xna.Framework;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class MultiPageTab : PageContentTab
    {
        private readonly TabViewport viewport;
        private readonly TabWindow window;

        public override ITabViewport TabViewport => viewport;
        public override PopWindow Window => window;

        public ObservableCollection<NotePage> NotePages { get; }

        public MultiPageTab(Point size)
        {
            NotePages = new ObservableCollection<NotePage>();

            viewport = new TabViewport { UISize = size };
            window = new TabWindow(size, viewport, NotePages);

            NotePages.CollectionChanged += (sender, _) => viewport.PageCount = ((ObservableCollection<NotePage>)sender).Count;
        }

        public override void AddContent(IPageContent content)
        {
            int focusedPageIndex = viewport.FocusedPageIndex;

            Point location = viewport.ScrolledDistance - focusedPageIndex * viewport.MovingDimensionPageStep;
            location += new Point(50);

            content.Location = location;
            NotePages[focusedPageIndex].Contents.Add(content);
        }

        public override Point GetContentScreenLocation(IPageContent content)
        {
            int contentPageIndex = -1;
            for (int i = 0; i < NotePages.Count; i++)
            {
                if (NotePages[i].Contents.Contains(content))
                {
                    contentPageIndex = i;
                    break;
                }
            }

            if (contentPageIndex == -1)
            {
                throw new ArgumentException("Content does not exist in any tab.");
            }

            return viewport.FirstPageStartPosition + contentPageIndex * viewport.MovingDimensionPageStep.Times(viewport.Zoom) + content.Location.Times(viewport.Zoom);
        }

        public override void ShellModeUpdated(ShellModes mode)
        {
            window.ModeUpdated(mode);
        }

        public override void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e)
        {
            window.SimulateMouseButtonStateInput(e);
        }
    }
}
