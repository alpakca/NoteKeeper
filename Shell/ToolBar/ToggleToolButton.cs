using Microsoft.Xna.Framework.Graphics;
using System;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToggleToolButton : IToolButton
    {
        private bool _isChecked;
        private readonly Action<ToggleToolButton> _toggleAction;

        public bool Enabled { get; set; } = true;
        public bool IsChecked => _isChecked;

        public Texture2D CheckedTexture { get; set; }
        public Texture2D UncheckedTexture { get; set; }
        public Texture2D UnenabledTexture { get; set; }

        public ToggleToolButton(Action<ToggleToolButton> toggleAction)
        {
            _toggleAction = toggleAction;
        }

        public Texture2D GetTexture(bool isHovered, bool isHeld)
        {
            if (!Enabled)
            {
                return UnenabledTexture;
            }

            return IsChecked ? CheckedTexture : UncheckedTexture;
        }

        public void Selected()
        {
            _isChecked = !_isChecked;
            _toggleAction(this);
        }
    }
}
