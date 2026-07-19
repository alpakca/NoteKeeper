using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

#nullable enable

namespace NoteKeeper.InspectionWindow.PagedTabs.SinglePage
{
    internal class ContentTransformer
    {
        public const int NodeLength = 10;

        private int transformedContentPageIndex;
        private IPageContent? transformedContent;

        private NodePlacement selectedNode;
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

        public ContentTransformer()
        {
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

        private NodePlacement GetHitNode(Point pagePosition)
        {
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
            AlignNodePositions();
        }

        public bool HitAnyNode(Point pagePosition)
        {
            return GetHitNode(pagePosition) != NodePlacement.None;
        }

        public void MouseHoldStart(Point pagePosition)
        {
            if (TransformedContent == null)
            {
                selectedNode = NodePlacement.None;
                return;
            }

            selectedNode = GetHitNode(pagePosition);
            lastMousePosition = pagePosition;
        }

        public void MouseHoldEnd()
        {
            selectedNode = NodePlacement.None;
        }

        public void MouseMove(Point pagePosition)
        {
            if (selectedNode == NodePlacement.None)
            {
                return;
            }

            Point difference = pagePosition - lastMousePosition;
            lastMousePosition = pagePosition;

            if (selectedNode == NodePlacement.Carry)
            {
                transformedContent!.Location += difference;
                AlignNodePositions();
                return;
            }

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

            transformedContent!.Location += locationChange;
            transformedContent.Size += sizeChange;
            AlignNodePositions();
        }

        private void AlignNodePositions()
        {
            if (transformedContent == null) return;

            int halfVertical = transformedContent.Size.Y / 2;
            int halfHorizontal = transformedContent.Size.X / 2;
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
