using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal interface IKeyTextReceiverContent
    {
        public void InsertText(string text);
        public void InsertChar(char ch);
        public void KeyPressed(Keys key);
        public void MouseSelected(Point mousePosition);
        public void InputEnded();
    }
}
