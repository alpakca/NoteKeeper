using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.UndoRedo
{
    internal class HistoryActionsStyleProvider
    {
        private HistoryActionsStyle _style;

        public HistoryActionsStyle Style => _style;

        public void ApplyTheme(HistoryActionsStyle style)
        {
            _style = style;
        }
    }
}
