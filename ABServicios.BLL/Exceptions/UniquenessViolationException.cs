using System;
using System.Data.SqlClient;

namespace ABServicios.BLL.Exceptions
{
    public class UniquenessViolationException : Exception
    {
			public UniquenessViolationException(string message, SqlException innerException) : base(message, innerException) { }
    }
}
