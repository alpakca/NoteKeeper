using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow.MultiPage.Components;
using NoteKeeper.InspectionWindow.MultiPage.History;
using PopUpUI.Text;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.InspectionWindow.MultiPage.Input
{
    internal class WritingSession
    {
        private readonly PageActions _pageActions;
        private readonly Dictionary<IHistoryAction, (TextPosition Before, TextPosition After)> _caretSnapshots;

        public IKeyTextReceiverContent? WritingContent { get; set; }
        public TextPosition CaretPosition { get; set; }

        public int LineSpacing => WritingContent!.LastResult.Data.LineSpacing;
        public Point CaretContentPosition => WritingContent!.LastResult.Caret.ScreenCaretPosition(CaretPosition, null).ToPoint();

        public WritingSession(PageActions pageActions)
        {
            _pageActions = pageActions;
            _pageActions.CommandApplied += PageActions_CommandApplied;

            _caretSnapshots = new Dictionary<IHistoryAction, (TextPosition Before, TextPosition After)>();
        }

        private void PageActions_CommandApplied(IHistoryAction appliedCommand, bool isUndo)
        {
            if (_caretSnapshots.TryGetValue(appliedCommand, out var snapshot))
            {
                CaretPosition = isUndo ? snapshot.Before : snapshot.After;
            }
            else if (WritingContent != null)
            {
                // clamp in case of out of bounds
                CaretPosition = WritingContent.LastResult.Caret.Clamp(CaretPosition);
            }
        }

        public void InsertChar(char ch)
        {
            if (WritingContent != null)
            {
                string oldText = WritingContent.Text;
                TextPosition oldCaret = CaretPosition;
                Point oldSize = WritingContent.Size;

                WritingContent.InsertChar(ch, this);

                string newText = WritingContent.Text;
                TextPosition newCaret = CaretPosition;
                Point newSize = WritingContent.Size;

                IHistoryAction action;
                if (newText != oldText && newSize != oldSize)
                {
                    action = new CombinedAction(new PropertyChangeAction<IKeyTextReceiverContent, string>(oldText, newText, WritingContent, (element, value) => element.Text = value),
                                                new PropertyChangeAction<IKeyTextReceiverContent, Point>(oldSize, newSize, WritingContent, (element, value) => element.SetSize(value)));
                }
                else
                {
                    action = new PropertyChangeAction<IKeyTextReceiverContent, string>(oldText, newText, WritingContent, (element, value) => element.Text = value);
                }

                _pageActions.RunAction(action);
                _caretSnapshots.Add(action, (oldCaret, newCaret));
            }
        }

        public void KeyInput(Keys key)
        {
            if (WritingContent != null)
            {
                WritingContent.KeyPressed(key, this);
            }
        }
    }
}
