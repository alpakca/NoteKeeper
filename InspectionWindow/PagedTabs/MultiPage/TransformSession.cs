using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow.PagedTabs.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class TransformSession
    {
        private NotePagePosition _holdOrigin;
        private Rectangle _initialBounds;

        private State _state;
        private TransformNode _activeNode;

        private readonly List<IPageContent> _selectedContents;
        private readonly List<ContentRelative> _relativeContents;

        private int _marqueePageIndex;
        private Rectangle _marqueeRectangle;

        private readonly PageActions _pageActions;
        private readonly TabViewport _viewport;
        private readonly IList<NotePage> _notePages;
        private readonly Dictionary<TransformNode, Point> _nodePagePositions;

        private enum State
        {
            Idle,
            Marqueeing,
            Transforming
        }

        private enum TransformNode
        {
            None,
            Carry,
            
            Top,
            Right,
            Bottom,
            Left,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private struct ContentRelative
        {
            public IPageContent Content;
            public double RelX;
            public double RelY;
            public double RelWidth;
            public double RelHeight;
        }

        public int NodeHitRadius { get; set; } = 6;
        public bool LockAspectRatioOnCornerDrag { get; set; } = true;
        public Rectangle SelectionBoundary { get; private set; }

        public bool NotIdle => _state != State.Idle || _selectedContents.Count != 0;
        public int ContentsPageIndex => _marqueePageIndex;
        public IEnumerable<Point> NodePagePositions => _nodePagePositions.Values;

        public TransformSession(PageActions pageActions, TabViewport viewport, IList<NotePage> notePages)
        {
            _nodePagePositions = new Dictionary<TransformNode, Point>(9);
            _selectedContents = new List<IPageContent>();
            _relativeContents = new List<ContentRelative>();

            _pageActions = pageActions;
            _viewport = viewport;
            _notePages = notePages;
        }

        public void HoldStart(NotePagePosition position)
        {
            _holdOrigin = position;

            if (_selectedContents.Count > 0 && TryHitNode(position, out TransformNode hit))
            {
                BeginTransforming(hit);
                return;
            }

            // there isn't a selected content or mouse did not hit any node
            DiscardSelection();
            _state = State.Marqueeing; // begin selection box
            _marqueeRectangle = new Rectangle(position.PagePosition, default);
            _marqueePageIndex = position.PageIndex;
            SelectionBoundary = _marqueeRectangle;
        }

        private void BeginTransforming(TransformNode hitNode)
        {
            _activeNode = hitNode;
            _initialBounds = SelectionBoundary;
            _state = State.Transforming;

            double width = _initialBounds.Width;
            double height = _initialBounds.Height;

            _relativeContents.Clear();
            foreach (var content in _selectedContents)
            {
                _relativeContents.Add(new ContentRelative
                {
                    Content = content,
                    RelX = (content.Location.X - _initialBounds.X) / width,
                    RelY = (content.Location.Y - _initialBounds.Y) / height,
                    RelWidth = content.Size.X / width,
                    RelHeight = content.Size.Y / height
                });
            }
        }

        public void HoldUpdate(NotePagePosition position)
        {
            if (_state == State.Marqueeing)
            {
                UpdateMarquee(position);
            }
            else if (_state == State.Transforming)
            {
                UpdateTransforming(position);
            }
        }

        private void UpdateMarquee(NotePagePosition position)
        {
            Point size = DifferenceBetweenOrigin(position);
            _marqueeRectangle = new Rectangle(_holdOrigin.PagePosition, size);

            SelectionBoundary = _marqueeRectangle;
            BuildNodePositions(_marqueeRectangle);
        }
        
        private void UpdateTransforming(NotePagePosition position)
        {
            Rectangle newBoundary = ComputeNewBoundary(position);

            SelectionBoundary = newBoundary;
            BuildNodePositions(newBoundary);

            foreach (var relative in _relativeContents)
            {
                Point newLocation = ComputeAbsoluteLocation(relative.RelX, relative.RelY, newBoundary);
                Point newSize = ComputeAbsoluteSize(relative.RelWidth, relative.RelHeight, newBoundary);

                relative.Content.RelocationUpdate(newLocation);
                relative.Content.ResizeUpdate(newSize);
            }
        }

        public void HoldEnded(NotePagePosition position)
        {
            if (_state == State.Marqueeing)
            {
                EndMarquee(position);
            } 
            else if (_state == State.Transforming)
            {
                EndTransforming(position);
            }

            _state = State.Idle;
        }

        private void EndMarquee(NotePagePosition position)
        {
            Point size = DifferenceBetweenOrigin(position);
            _marqueeRectangle = new Rectangle(_holdOrigin.PagePosition, size);

            GetContentsInRect(_marqueeRectangle);
            if (_selectedContents.Count == 0)
            {
                DiscardSelection();
                return;
            }

            Rectangle boundary = GetContentsBoundary(_selectedContents);
            
            if (boundary.Width == 0 || boundary.Height == 0)
            {
                DiscardSelection();
                return;
            }

            SelectionBoundary = boundary;
            BuildNodePositions(boundary);
        }

        private void EndTransforming(NotePagePosition position)
        {
            Rectangle newBoundary = ComputeNewBoundary(position);

            SelectionBoundary = newBoundary;
            BuildNodePositions(newBoundary);

            int index = 0;
            IHistoryAction[] contentActions;

            // may only happen using TransformNode.Carry, which does not effect size
            if (position.PageIndex != _marqueePageIndex)
            {
                contentActions = new IHistoryAction[_relativeContents.Count * 2];

                Point pageIndexOffset = (_marqueePageIndex - position.PageIndex) * _viewport.MovingDimensionPageStep;
                NotePage oldPage = _notePages[_marqueePageIndex];
                NotePage newPage = _notePages[position.PageIndex];

                foreach (var relative in _relativeContents)
                {
                    Point oldLocation = ComputeAbsoluteLocation(relative.RelX, relative.RelY, _initialBounds);
                    Point newLocation = ComputeAbsoluteLocation(relative.RelX, relative.RelY, newBoundary) + pageIndexOffset;

                    contentActions[index++] = new PropertyChangeAction<IPageContent, Point>(oldLocation, newLocation, relative.Content, (e, v) => e.SetLocation(v));
                    contentActions[index++] = new MoveContentAcrossPageAction(oldPage, newPage, relative.Content);
                }
            }
            else
            {
                contentActions = new IHistoryAction[_relativeContents.Count];

                foreach (var relative in _relativeContents)
                {
                    Point oldLocation = ComputeAbsoluteLocation(relative.RelX, relative.RelY, _initialBounds);
                    Point oldSize = ComputeAbsoluteSize(relative.RelWidth, relative.RelHeight, _initialBounds);

                    Point newLocation = ComputeAbsoluteLocation(relative.RelX, relative.RelY, newBoundary);
                    Point newSize = ComputeAbsoluteSize(relative.RelWidth, relative.RelHeight, newBoundary);

                    contentActions[index++] = new PropertyChangeAction<IPageContent, Rectangle>(
                        new Rectangle(oldLocation, oldSize), 
                        new Rectangle(newLocation, newSize), 
                        relative.Content, (e, v) =>
                        {
                            e.SetLocation(v.Location);
                            e.SetSize(v.Size);
                        });
                }
            }

            _pageActions.CombinedAction(contentActions);

            // display the marquee & initial bounds to as they appear in new page index
            // done here because we get the new boundary from contents
            // which is applied after pageAction runs the property change
            if (position.PageIndex != _marqueePageIndex)
            {
                _marqueePageIndex = position.PageIndex;

                SelectionBoundary = GetContentsBoundary(_selectedContents);
                _initialBounds = SelectionBoundary;
                BuildNodePositions(SelectionBoundary);
            }

            _activeNode = TransformNode.None;
        }

        private Rectangle ComputeNewBoundary(NotePagePosition position)
        {
            (int dx, int dy) = DifferenceBetweenOrigin(position);

            if (_activeNode == TransformNode.Carry)
            {
                return new Rectangle(_initialBounds.X + dx, _initialBounds.Y + dy, _initialBounds.Width, _initialBounds.Height);
            }

            return _activeNode switch
            {
                TransformNode.Carry => new Rectangle(_initialBounds.X + dx, _initialBounds.Y + dy, _initialBounds.Width, _initialBounds.Height),

                TransformNode.Top => new Rectangle(_initialBounds.X, _initialBounds.Y + dy, _initialBounds.Width, _initialBounds.Height - dy),
                TransformNode.Right => new Rectangle(_initialBounds.X, _initialBounds.Y, _initialBounds.Width + dx, _initialBounds.Height),
                TransformNode.Bottom => new Rectangle(_initialBounds.X, _initialBounds.Y, _initialBounds.Width, _initialBounds.Height + dy),
                TransformNode.Left => new Rectangle(_initialBounds.X + dx, _initialBounds.Y, _initialBounds.Width - dx, _initialBounds.Height),

                
                TransformNode.TopLeft => ComputeCornerDragBoundary(dx, dy, isLeft: true, isTop: true),
                TransformNode.TopRight => ComputeCornerDragBoundary(dx, dy, isLeft: false, isTop: true),
                TransformNode.BottomLeft => ComputeCornerDragBoundary(dx, dy, isLeft: true, isTop: false),
                TransformNode.BottomRight => ComputeCornerDragBoundary(dx, dy, isLeft: false, isTop: false),
                
                _ => _initialBounds
            };
        }

        private Rectangle ComputeCornerDragBoundary(int dx, int dy, bool isLeft, bool isTop)
        {
            double rawWidth = isLeft ? _initialBounds.Width - dx : _initialBounds.Width + dx;
            double rawHeight = isTop ? _initialBounds.Height - dy : _initialBounds.Height + dy;

            double newWidth;
            double newHeight;

            if (LockAspectRatioOnCornerDrag)
            {
                double scaleX = rawWidth / _initialBounds.Width;
                double scaleY = rawHeight / _initialBounds.Height;

                // both axis change size in dominant axis
                double uniformScale = Math.Abs(scaleX - 1.0) >= Math.Abs(scaleY - 1.0)
                    ? scaleX
                    : scaleY;

                newWidth = _initialBounds.Width * uniformScale;
                newHeight = _initialBounds.Height * uniformScale;
            }
            else
            {
                // acts as independent two nodes
                newWidth = rawWidth;
                newHeight = rawHeight;
            }

            // The fixed corner position
            int anchorX = isLeft ? _initialBounds.Right : _initialBounds.X;
            int anchorY = isTop ? _initialBounds.Bottom : _initialBounds.Y;

            int roundedWidth = RoundToInt(newWidth);
            int roundedHeight = RoundToInt(newHeight);

            int newX = isLeft ? anchorX - roundedWidth : anchorX;
            int newY = isTop ? anchorY - roundedHeight : anchorY;

            return new Rectangle(newX, newY, roundedWidth, roundedHeight);
        }

        private void DiscardSelection()
        {
            _relativeContents.Clear();
            _selectedContents.Clear();
            _nodePagePositions.Clear();
            _state = State.Idle;
            SelectionBoundary = default;
            _activeNode = TransformNode.None;
        }

        private bool TryHitNode(NotePagePosition position, out TransformNode hit)
        {
            int tolearanceArea = NodeHitRadius * NodeHitRadius;
            Point adjusted = position.PagePosition + (position.PageIndex - _marqueePageIndex) * _viewport.MovingDimensionPageStep;

            foreach (var (node, placement) in _nodePagePositions)
            {
                int dx = adjusted.X - placement.X;
                int dy = adjusted.Y - placement.Y;

                if (dx * dx + dy * dy <= tolearanceArea)
                {
                    hit = node;
                    return true;
                }
            }

            hit = TransformNode.None;
            return false;
        }

        private Rectangle GetContentsBoundary(List<IPageContent> contents)
        {
            int minX = int.MaxValue; int minY = int.MaxValue;
            int maxX = int.MinValue; int maxY = int.MinValue;

            foreach (var content in contents)
            {
                minX = Math.Min(minX, content.Location.X);
                minY = Math.Min(minY, content.Location.Y);
                maxX = Math.Max(maxX, content.Location.X + content.Size.X);
                maxY = Math.Max(maxY, content.Location.Y + content.Size.Y);
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private void GetContentsInRect(Rectangle rect)
        {
            rect = rect.Normalized();
            _selectedContents.Clear();

            foreach (var content in _notePages[_marqueePageIndex].Contents)
            {
                Rectangle intersect = Rectangle.Intersect(new Rectangle(content.Location, content.Size).Normalized(), rect);
                float fillPercentage = (float)(intersect.Width * intersect.Height) / (content.Size.X * content.Size.Y) * 100;

                if (fillPercentage > 50)
                {
                    _selectedContents.Add(content);
                }
            }
        }

        private void BuildNodePositions(Rectangle boundary)
        {
            Point center = boundary.Center;
            int halfHorizontal = boundary.Width / 2;
            int halfVertical = boundary.Height / 2;

            _nodePagePositions[TransformNode.TopLeft] = center + new Point(-halfHorizontal, -halfVertical);
            _nodePagePositions[TransformNode.TopRight] = center + new Point(halfHorizontal, -halfVertical);
            _nodePagePositions[TransformNode.BottomLeft] = center + new Point(-halfHorizontal, halfVertical);
            _nodePagePositions[TransformNode.BottomRight] = center + new Point(halfHorizontal, halfVertical);
            _nodePagePositions[TransformNode.Carry] = center;
            _nodePagePositions[TransformNode.Top] = center + new Point(0, -halfVertical);
            _nodePagePositions[TransformNode.Left] = center + new Point(-halfHorizontal, 0);
            _nodePagePositions[TransformNode.Right] = center + new Point(halfHorizontal, 0);
            _nodePagePositions[TransformNode.Bottom] = center + new Point(0, halfVertical);
        }

        private Point DifferenceBetweenOrigin(NotePagePosition position)
        {
            return position.PagePosition + (position.PageIndex - _holdOrigin.PageIndex) * _viewport.MovingDimensionPageStep - _holdOrigin.PagePosition;
        }

        private static Point ComputeAbsoluteLocation(double relX, double relY, Rectangle boundary)
        {
            double x = boundary.X + relX * boundary.Width;
            double y = boundary.Y + relY * boundary.Height;
            return new Point(RoundToInt(x), RoundToInt(y));
        }

        private static Point ComputeAbsoluteSize(double relW, double relH, Rectangle boundary)
        {
            double w = relW * boundary.Width;
            double h = relH * boundary.Height;
            return new Point(RoundToInt(w), RoundToInt(h));
        }

        private static int RoundToInt(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
