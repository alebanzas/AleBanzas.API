using System;
using System.Data.SqlClient;

namespace ABServicios.BLL.Exceptions
{
    public class ConstraintViolationException : Exception
    {
			public ConstraintViolationException(string message, SqlException innerException) : base(message, innerException) { }
    }
}
