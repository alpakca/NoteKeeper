using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoteKeeper.Shell.UndoRedo
{
    internal class HistoryActionsManager
    {
        private readonly HistoryActionsRenderer renderer;
        private readonly HistoryActionsLayoutCalculator layoutCalculator;

        private IEnumerable<IHistoryAction> _actions;
        private int _hoveredItemIndex;
        private bool _itemHeld;

        public HistoryActionsStyleProvider StyleProvider { get; }

        public event Action<int> ActionsReversed;

        public HistoryActionsManager()
        {
            StyleProvider = new HistoryActionsStyleProvider();

            layoutCalculator = new HistoryActionsLayoutCalculator(StyleProvider);
            renderer = new HistoryActionsRenderer(StyleProvider, layoutCalculator);
        }

        public void Show(IEnumerable<IHistoryAction> actions, Point screenLocation)
        {
            Hide();

            _actions = actions;
            layoutCalculator.ScreenLocation = screenLocation;
            layoutCalculator.CalculateActionsWidth(actions);
        }

        public void Hide()
        {
            _actions = null;
            _hoveredItemIndex = -1;
        }

        public void Render()
        {
            if (_actions != null)
            {
                renderer.Render(_actions, _hoveredItemIndex);
            }
        }

        public void MousePositionChangeUpdate(PopWindowMouseEventArgs e)
        {
            if (_actions == null)
                return;

            int count = _actions.Count();
            for (int i = count - 1; i >= 0; i--)
            {
                if (layoutCalculator.GetActionLayout(i).Contains(e.MousePosition))
                {
                    _hoveredItemIndex = i;
                    break;
                }
            }
        }

        public void MouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (_actions == null)
                return;

            if (!layoutCalculator.BackgroundBounds(_actions.Count()).Contains(e.MousePosition))
            {
                Hide();
                return;
            }

            if (e.FirstTimeButtons.HasLeft && _hoveredItemIndex != -1)
            {
                _itemHeld = true;
            }
            else if (e.ReleasedButtons.HasLeft)
            {
                if (_itemHeld && _hoveredItemIndex != -1)
                {
                    ActionsReversed?.Invoke(_hoveredItemIndex + 1); // returns the number of items, so add one
                }

                _itemHeld = false;
            }
        }
    }
}
