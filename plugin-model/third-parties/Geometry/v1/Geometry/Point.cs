namespace Geometry;

public record Point(double X, double Y, double Z);

public static class PointCreation
{
    extension(Point)
    {
        public static Point FromXYZ(double x, double y, double z)
        {
            return new Point(x, y, z);
        }
    }
}
