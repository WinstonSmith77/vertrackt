using System;

namespace Vertrackt.Geometry
{
    public class BoundingBox : IBoundingBox
    {
        private readonly int _left;
        private readonly int _right;
        private readonly int _bottom;
        private readonly int _top;

        public BoundingBox(Point a, Point b)
        {
            _left = Math.Min(a.X, b.X);
            _right = Math.Max(a.X, b.X);
            _bottom = Math.Min(a.Y, b.Y);
            _top = Math.Max(a.Y, b.Y);
        }

        private BoundingBox(int left, int right, int bottom, int top)
        {
            _left = left;
            _right = right;
            _bottom = bottom;
            _top = top;
        }

        public bool IsInside(Point p)
        {
            return p.X >= _left && p.X <= _right && p.Y >= _bottom && p.Y <= _top;
        }

        public IBoundingBox Inflate(int by)
        {
            return Inflate(by, by, by, by);
        }

        public IBoundingBox Inflate(int byLeft, int byRight, int byBottom, int byTop)
        {
            return new BoundingBox(_left - byLeft, _right + byRight, _bottom - byBottom, _top + byTop);
        }

        public IBoundingBox ScaleDown(int scale)
        {
            return new BoundingBox(_left / scale, _right / scale, _bottom / scale, _top / scale);
        }

        public IBoundingBox ScaleUp(int scale)
        {
            return new BoundingBox(_left * scale, _right * scale, _bottom * scale, _top * scale);
        }
    }
}
