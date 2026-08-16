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

        public event Action<IEnumerable<IHistoryAction>, int> ActionsReversed;

        public HistoryActionsManager()
        {
            layoutCalculator = new HistoryActionsLayoutCalculator();
            renderer = new HistoryActionsRenderer(layoutCalculator);
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

            _hoveredItemIndex = GetHoveredItemIndex(e.MousePosition);
        }

        private int GetHoveredItemIndex(Point mousePosition)
        {
            int count = _actions.Count();
            for (int i = count - 1; i >= 0; i--)
            {
                if (layoutCalculator.GetActionLayout(i).Contains(mousePosition))
                {
                    return i;
                }
            }

            return -1;
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
                    ActionsReversed?.Invoke(_actions, _hoveredItemIndex + 1); // returns the number of items, so add one
                    Hide();
                }

                _itemHeld = false;
            }
        }
    }
}
