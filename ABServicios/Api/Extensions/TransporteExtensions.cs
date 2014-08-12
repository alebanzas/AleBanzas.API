using System.Collections.Generic;
using AB.Common.Helpers;
using ABServicios.Api.Models;
using ABServicios.BLL.Entities;

namespace ABServicios.Api.Extensions
{
    public static class TransporteExtensions
    {
        public static TransporteViewModel ToTransporteViewModel(this Transporte transporte, bool incluyePuntos = false)
        {
            return new TransporteViewModel
            {
                ID = transporte.ID,
                TipoNickName = transporte.Tipo.Nombre.ToUrl(),
                Nombre = string.Concat(transporte.Nombre, " ", transporte.Regreso ? " (ida)" : " (vuelta)"),
                Linea = transporte.Linea,
                Ramal = transporte.Ramal,
                Puntos = incluyePuntos ? transporte.Ubicacion.Coordinates.ToPuntoViewModel() : new List<PuntoViewModel>(),
                RecorridoText = transporte.RecorridoText,
            };
        }
    }
}