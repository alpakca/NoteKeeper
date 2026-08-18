using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.Shell.Styling;
using PopUpUI.Elements;
using PopUpUI.Input;
using System;
using System.Text;

namespace NoteKeeper.Shell.ChildWindow
{
    internal class LinearTextBox : PopElement, ISelectable, IKeyReceiver, ITextReceiver
    {
        private string _text = "";
        private int _caretPosition = 0;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _caretPosition = Math.Clamp(_caretPosition, 0, _text.Length);
            }
        }
        public int CaretOffset => (int)FontProvider.MeasureWidth(TextSize, IsTextBold, _text[.._caretPosition]);

        public int TextSize { get; set; }
        public bool IsTextBold { get; set; }
        public Rectangle SelectableArea { get; set; }

        public bool ReceivesInput { get; set; }

        public SelectablePacket SelectablePacket { get; set; }
        public KeyReceiverPacket KeyReceiverPacket { get; set; }
        public TextReceiverPacket TextReceiverPacket { get; set; }

        bool IKeyReceiver.ReceivesKeyInput => ReceivesInput;
        bool ITextReceiver.ReceivesTextInput => ReceivesInput;

        public void OnTextInputReceive(PopWindowMouseEventArgs e, char ch)
        {
            if (ch == '\n' || !FontProvider.CanCharacterResolved(TextSize, IsTextBold, ch))
            {
                return;
            }    

            _text = _text.Insert(_caretPosition, ch.ToString());
            _caretPosition += 1;
        }

        public void Backspace()
        {
            if (_text.Length == 0 || _caretPosition == 0)
                return;

            _text = _text[..(_caretPosition - 1)] + _text[_caretPosition..];
            _caretPosition -= 1;
        }

        public void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            switch (key)
            {
                case Keys.Up:

                    _caretPosition = 0;
                    break;
                case Keys.Down:

                    _caretPosition = _text.Length;
                    break;
                case Keys.Left:

                    _caretPosition = Math.Max(0, _caretPosition - 1);
                    break;
                case Keys.Right:

                    _caretPosition = Math.Min(_text.Length, _caretPosition + 1);
                    break;

                case Keys.Back:

                    Backspace();
                    break;
            }
        }

        public bool CanBeSelected(Point point)
        {
            return SelectableArea.Contains(point);
        }

        public void OnSelect(PopWindowMouseEventArgs e)
        {
            if (_text.Length == 0)
            {
                _caretPosition = 0;
                return;
            }

            Point inTextBoxPosition = e.MousePosition - SelectableArea.Location;

            // input inside box will be single line, so we can directly pass zero as Y value
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < _text.Length - 1; i++)
            {
                if (FontProvider.MeasureWidth(TextSize, IsTextBold, builder) 
                  + FontProvider.MeasureWidth(TextSize, IsTextBold, _text[i].ToString()) >= inTextBoxPosition.X)
                {
                    _caretPosition = i;
                    return;
                }

                builder.Append(_text[i]);
            }

            _caretPosition = _text.Length;
        }

        public void OnSelectingOutside(PopWindowMouseEventArgs e)
        {
        }

        public void OnKeyUp(PopWindowMouseEventArgs e, Keys key)
        {
        }
    }
}
