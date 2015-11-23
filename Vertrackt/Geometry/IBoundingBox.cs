namespace Vertrackt.Geometry
{
    public interface IBoundingBox : IScaleDown<IBoundingBox>
    {
        bool IsInside(Point p);
    }
}