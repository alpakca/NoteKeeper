using Microsoft.Xna.Framework;
using PopUpUI.PUI;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;

namespace NoteKeeper.Shell.PUIHelpers
{
    public class AnimationTupleParser : ITupleParser
    {
        public string TypeName => "Animation";

        public object Parse(object[] values)
        {
            if (values.Length != 1 || values[0] is not string path)
            {
                throw new TupleParseException("Animation tuple parser takes single string argument");
            }

            return Animation2D.FromTexture(DrawHelper.FromFile(path));
        }
    }

    public class RectangleTupleParser : ITupleParser
    {
        public string TypeName => "Rectangle";

        public object Parse(object[] values)
        {
            if (values.Length != 4)
                throw new TupleParseException("Animation tuple parser takes 4 int argument");

            return new Rectangle((int)values[0], (int)values[1], (int)values[2], (int)values[3]);
        }
    }

    public class PointTupleParser : ITupleParser
    {
        public string TypeName => "Point";

        public object Parse(object[] values)
        {
            if (values.Length != 2)
                throw new TupleParseException("Animation tuple parser takes 4 int argument");

            return new Point((int)values[0], (int)values[1]);
        }
    }
}
