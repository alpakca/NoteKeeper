using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell
{
    internal static class DrawCallRenderer
    {
        private static readonly Queue<Action> _drawCalls = new Queue<Action>();

        public static void Enqueue(Action drawAction)
        {
            _drawCalls.Enqueue(drawAction);
        }

        public static void DrawUpdate()
        {
            if (_drawCalls.Count == 0)
                return;

            Action drawAction = _drawCalls.Dequeue();
            drawAction.Invoke();
        }
    }
}
