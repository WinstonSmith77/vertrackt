using System;

namespace Vertrackt.Geometry
{
    public class BoundingBox
    {
        private readonly int left;
        private readonly int right;
        private readonly int bottom;
        private readonly int top;

        public BoundingBox(Point a, Point b)
        {
            left = Math.Min(a.X, b.X);
            right = Math.Max(a.X, b.X);
            bottom = Math.Min(a.Y, b.Y);
            top = Math.Max(a.Y, b.Y);
        }

        public bool IsInside(Point p)
        {
            return p.X >= left && p.X <= right && p.Y >= bottom && p.Y <= top;
        }
    }
}
