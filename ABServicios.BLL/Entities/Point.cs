using System;

namespace ABServicios.BLL.Entities
{
    public class Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public double Distance(Point ubicacion)
        {
            return Math.Abs(ubicacion.X - X) + Math.Abs(ubicacion.Y - Y);
        }
    }
}