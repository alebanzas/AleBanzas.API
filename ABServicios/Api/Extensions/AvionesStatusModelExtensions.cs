using ABServicios.Models;

namespace ABServicios.Api.Extensions
{
    public static class AvionesStatusModelExtensions
    {
        public static AvionesTerminalStatusModel ToArribos(this AvionesTerminalStatusModel source)
        {
            return new AvionesTerminalStatusModel
            {
                Actualizacion = source.Actualizacion,
                NickName = source.NickName,
                Arribos = source.Arribos,
                Nombre = source.Nombre,
            };
        }

        public static AvionesTerminalStatusModel ToPartidas(this AvionesTerminalStatusModel source)
        {
            return new AvionesTerminalStatusModel
            {
                Actualizacion = source.Actualizacion,
                NickName = source.NickName,
                Partidas = source.Partidas,
                Nombre = source.Nombre,
            };
        }
    }
}