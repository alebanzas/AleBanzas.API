using System;

namespace AB.Common.Helpers
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// A partir de un cualquier DateTime devuelve el pricipio del dia (hora 00:00:00.000)
        /// </summary>
        /// <param name="current">El timestamp actual o cualquier DateTime</param>
        /// <returns>El timestamp del inicio del dia</returns>
        public static DateTime BeginOfDay(this DateTime current)
        {
            var result = new DateTime(current.Year, current.Month, current.Day);
            return result;
        }

        /// <summary>
        /// A partir de un cualquier DateTime devuelve el final del dia (hora 23:59:59.996)
        /// </summary>
        /// <param name="current">El timestamp actual o cualquier DateTime</param>
        /// <returns>El timestamp del final del dia</returns>
        /// <remarks>
        /// Se usan 4ms porque MSSQL redondea a los 3ms
        /// </remarks>
        public static DateTime EndOfDay(this DateTime current)
        {
            DateTime result = (new DateTime(current.Year, current.Month, current.Day)).AddDays(1).AddMilliseconds(-4D);
            return result;
        }

        /// <summary>
        /// A partir de un cualquier DateTime devuelve el final del mes (hora 23:59:59.996)
        /// </summary>
        /// <param name="current">El timestamp actual o cualquier DateTime</param>
        /// <returns>El timestamp del final del ultimo dia del mes</returns>
        /// <remarks>
        /// Se usan 4ms porque MSSQL redondea a los 3ms
        /// </remarks>
        public static DateTime EndOfMonth(this DateTime current)
        {
            DateTime result = (new DateTime(current.Year, current.Month, 1)).AddMonths(1).AddMilliseconds(-4D);
            return result;
        }
    }
}