using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Elements;
using PopUpUI.Surface;
using PopUpUI.Text;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell.ChildWindow
{
    internal class TextPlaque : PopElement, IDisposable
    {
        private readonly TextureDrawInfo _drawInfo;

        private bool _textureDirty;
        private RenderTarget2D _target;
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

                    CreateTargetIfNeeded();
                    _textureDirty = true;
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

                    CreateTargetIfNeeded();
                    _textureDirty = true;
                }
            }
        }

        public override Rectangle Boundaries 
        {
            set
            {
                if (value.Size.X != 0 && value.Size.Y != 0 && this.Boundaries.Size != value.Size && Processer != null)
                {
                    Processer.HorizontalTextLength = value.X;
                    _lastResult = Processer.GetParseResult();

                    CreateTargetIfNeeded();
                }

                Rectangle newBounds = new Rectangle(value.Location, _lastResult.TextSize.ToPoint());
                _drawInfo.DestinationArea = newBounds;
                base.Boundaries = newBounds;
            } 
        }

        public TextPlaque()
        {
            _drawInfo = new TextureDrawInfo();
        }

        public override void RenderUpdate()
        {
            if (_textureDirty)
            {
                _lastResult.Render(_target, BatchDrawOptions.Default, default, 1f);
                _textureDirty = false;

                _drawInfo.Texture = _target;
            }

            base.RenderUpdate();
        }

        public override IEnumerable<UIDrawInfo> GetDrawInfos()
        {
            return [_drawInfo];
        }

        public void Dispose()
        {
            _target?.Dispose();
            _drawInfo.Texture = null;
        }

        private void CreateTargetIfNeeded()
        {
            if (this._target?.Width != (int)_lastResult.TextSize.X ||
                this._target?.Width != (int)_lastResult.TextSize.Y)
            {
                _target?.Dispose();
                _target = DrawHelper.CreateRenderTarget((int)_lastResult.TextSize.X, (int)_lastResult.TextSize.Y);
            }
        }
    }
}
