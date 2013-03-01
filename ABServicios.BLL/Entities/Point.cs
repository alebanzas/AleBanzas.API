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
            return Distance(ubicacion, this);
        }

        public static double Distance(Point origen, Point destino)
        {
            return Math.Pow(origen.X - destino.X, 2) + Math.Pow(origen.Y - destino.Y, 2);
            //return Math.Abs(origen.X - destino.X) + Math.Abs(origen.Y - destino.Y);
        }
    }
}