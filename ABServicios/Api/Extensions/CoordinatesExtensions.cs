using System.Collections.Generic;
using System.Linq;
using ABServicios.Api.Models;
using GeoAPI.Geometries;

namespace ABServicios.Api.Extensions
{
    public static class CoordinatesExtensions
    {
        public static List<PuntoViewModel> ToPuntoViewModel(this Coordinate[] source)
        {
            return source.Select(x => new PuntoViewModel
            {
                X = x.X,
                Y = x.Y,
            }).ToList();
        }
    }
}