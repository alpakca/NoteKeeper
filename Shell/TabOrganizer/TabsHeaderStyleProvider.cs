using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabsHeaderStyleProvider
    {
        private TabsHeaderStyle _style;

        public TabsHeaderStyle Style => _style;

        public void ApplyTheme(TabsHeaderStyle style)
        {
            _style = style;
        }
    }
}
