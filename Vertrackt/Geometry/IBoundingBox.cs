namespace Vertrackt.Geometry
{
    public interface IBoundingBox
    {
        bool IsInside(Point p);
    }
}