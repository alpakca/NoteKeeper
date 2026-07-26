using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow.PagedTabs.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class ContentTransformer
    {
        public const int NodeLength = 10;

        private int transformedContentPageIndex;
        private IPageContent? transformedContent;

        private Point transformedContentStartLocation;
        private Point transformedContentStartSize;

        private Point cumulativeLocationChange;
        private Point cumulativeSizeChange;

        private readonly TabViewport _viewport;
        private readonly PageActions _pageActions;
        private readonly IList<NotePage> _notePages;

        private NodePlacement selectedNode;
        private int lastPageIndex;
        private Point lastMousePosition;
        private readonly Dictionary<NodePlacement, Point> nodePagePositions;

        public int TransformedContentPageIndex => transformedContentPageIndex;
        public IPageContent? TransformedContent => transformedContent;

        public bool AnyNodeSelected => selectedNode != NodePlacement.None;
        public bool IsTransforming => transformedContent != null;
        public IEnumerable<Point> NodePagePositions => nodePagePositions.Values;

        private enum NodePlacement
        {
            None = 0,
            Carry = 1,

            Top = 2,
            Bottom = 4,

            Left = 8,
            Right = 16
        }

        public ContentTransformer(IList<NotePage> notePages, TabViewport viewport, PageActions pageActions)
        {
            _notePages = notePages;
            _viewport = viewport;
            _pageActions = pageActions;

            nodePagePositions = new Dictionary<NodePlacement, Point>
            {
                { NodePlacement.Carry, default },
                { NodePlacement.Top | NodePlacement.Left, default }, { NodePlacement.Top, default },
                { NodePlacement.Top | NodePlacement.Right, default },
                { NodePlacement.Left, default }, { NodePlacement.Right, default },
                { NodePlacement.Bottom | NodePlacement.Left, default }, { NodePlacement.Bottom, default},
                { NodePlacement.Bottom | NodePlacement.Right, default }
            };
        }

        private NodePlacement GetHitNode(int pageIndex, Point pagePosition)
        {
            // if node that tried to hit is at another page (stretched to) than content, we need to adjust page position
            pagePosition += (pageIndex - transformedContentPageIndex) * _viewport.MovingDimensionPageStep;

            foreach ((NodePlacement node, Point position) in nodePagePositions)
            {
                if (position.X <= pagePosition.X && pagePosition.X < position.X + NodeLength &&
                    position.Y <= pagePosition.Y && pagePosition.Y < position.Y + NodeLength)
                {
                    return node;
                }
            }

            return NodePlacement.None;
        }

        public void SetTransformedContent(IPageContent? content, int contentPageIndex)
        {
            transformedContent = content;
            transformedContentPageIndex = contentPageIndex;

            if (transformedContent != null)
            {
                transformedContentStartLocation = transformedContent.Location;
                transformedContentStartSize = transformedContent.Size;

                cumulativeLocationChange = transformedContent.Location;
                cumulativeSizeChange = transformedContent.Size;
                AlignNodePositions();
            }
        }

        public bool HitAnyNode(int pageIndex, Point pagePosition)
        {
            return GetHitNode(pageIndex, pagePosition) != NodePlacement.None;
        }

        public void MouseHoldStart(int pageIndex, Point pagePosition)
        {
            if (transformedContent == null)
            {
                selectedNode = NodePlacement.None;
                return;
            }

            selectedNode = GetHitNode(pageIndex, pagePosition);
            lastPageIndex = pageIndex;
            lastMousePosition = pagePosition;

            transformedContentStartLocation = transformedContent.Location;
            transformedContentStartSize = transformedContent.Size;

            cumulativeLocationChange = transformedContent.Location;
            cumulativeSizeChange = transformedContent.Size;
        }

        public void MouseHoldEnd()
        {
            selectedNode = NodePlacement.None;

            _pageActions.ChangeContentProperty(new Rectangle(transformedContentStartLocation, transformedContentStartSize), 
                                               new Rectangle(cumulativeLocationChange, cumulativeSizeChange), transformedContent, 
                                               (element, value) =>
            {
                element!.SetLocation(value.Location);
                element.SetSize(value.Size);
            });

            transformedContentStartLocation = new Point();
            transformedContentStartSize = new Point();

            cumulativeLocationChange = new Point();
            cumulativeSizeChange = new Point();
        }

        public void MouseMove(int pageIndex, Point pagePosition)
        {
            if (selectedNode == NodePlacement.None || transformedContent == null)
            {
                return;
            }

            if (selectedNode == NodePlacement.Carry)
            {
                if (transformedContentPageIndex != pageIndex)
                {
                    _pageActions.MoveContentAcrossPages(_notePages[transformedContentPageIndex], _notePages[pageIndex], transformedContent);

                    transformedContentPageIndex = pageIndex;

                    // place it as node appears at the middle of content
                    Point newLocation = pagePosition - transformedContent.Size / new Point(2);

                    transformedContent.RelocationUpdate(newLocation - transformedContent.Location);
                    cumulativeLocationChange = newLocation;
                }
                else
                {
                    // carry by position change
                    Point carriedChange = pagePosition - lastMousePosition;

                    transformedContent.RelocationUpdate(carriedChange);
                    cumulativeLocationChange += carriedChange;
                }

                lastMousePosition = pagePosition;
                AlignNodePositions();
                return;
            }

            Point difference = pagePosition + (pageIndex - lastPageIndex) * _viewport.MovingDimensionPageStep - lastMousePosition;

            lastPageIndex = pageIndex;
            lastMousePosition = pagePosition;

            Point locationChange = new Point();
            Point sizeChange = new Point();

            if (selectedNode.HasFlag(NodePlacement.Top))
            {
                locationChange.Y += difference.Y;
                sizeChange.Y -= difference.Y;
            }
            else if (selectedNode.HasFlag(NodePlacement.Bottom))
            {
                sizeChange.Y += difference.Y;
            }

            if (selectedNode.HasFlag(NodePlacement.Left))
            {
                locationChange.X += difference.X;
                sizeChange.X -= difference.X;
            }
            else if (selectedNode.HasFlag(NodePlacement.Right))
            {
                sizeChange.X += difference.X;
            }

            transformedContent.RelocationUpdate(locationChange);
            transformedContent.ResizeUpdate(sizeChange);

            cumulativeLocationChange += locationChange;
            cumulativeSizeChange += sizeChange;
            AlignNodePositions();
        }

        private void AlignNodePositions()
        {
            if (transformedContent == null) return;

            int halfVertical = cumulativeSizeChange.Y / 2;
            int halfHorizontal = cumulativeSizeChange.X / 2;
            Point carryPosition = transformedContent.Location + new Point(halfHorizontal - NodeLength / 2, halfVertical - NodeLength / 2);

            foreach (NodePlacement node in nodePagePositions.Keys)
            {
                Point position = carryPosition;

                if (node.HasFlag(NodePlacement.Top))
                    position.Y -= halfVertical;

                else if (node.HasFlag(NodePlacement.Bottom))
                    position.Y += halfVertical;

                if (node.HasFlag(NodePlacement.Left))
                    position.X -= halfHorizontal;

                else if (node.HasFlag(NodePlacement.Right))
                    position.X += halfHorizontal;

                nodePagePositions[node] = position;
            }
        }
    }
}
