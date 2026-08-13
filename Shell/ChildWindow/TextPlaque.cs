using Microsoft.Xna.Framework;
using PopUpUI.Surface;
using PopUpUI.Text;

namespace NoteKeeper.Shell.ChildWindow
{
    internal class TextPlaque
    {
        private ITextProcesserResult _lastResult;

        public string Text
        {
            get;
            set
            {
                if (field == value)
                    return;

                field = value;
                if (Processer != null)
                {
                    Processer.Text = value;
                    _lastResult = Processer.GetParseResult();
                }
            }
        }
        public ITextProcesser Processer
        {
            get;
            set
            {
                field = value;

                if (this.Text != null)
                {
                    value.Text = this.Text;
                    _lastResult = value.GetParseResult();
                }
            }
        }

        public Point Location { get; set; }
        public int Width
        {
            get;
            set
            {
                if (value != 0 && field != value && Processer != null)
                {
                    Processer.HorizontalTextLength = value;
                    _lastResult = Processer.GetParseResult();
                }

                field = value;
            }
        }

        public void Render()
        {
            _lastResult.Render(BatchDrawOptions.Default, Location.ToVector2(), null, 1f);
        }
    }
}
