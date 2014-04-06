using System;
using System.Data.SqlClient;
using ABServicios.BLL.Exceptions;
using NHibernate.Exceptions;
using ConstraintViolationException = ABServicios.BLL.Exceptions.ConstraintViolationException;

namespace AB.Data
{
    public class MsSqlExceptionConverter : ISQLExceptionConverter
    {
        public Exception Convert(AdoExceptionContextInfo exInfo)
        {
            var sqle = ADOExceptionHelper.ExtractDbException(exInfo.SqlException) as SqlException;
            if (sqle != null)
            {
                switch (sqle.Number)
                {
                    case 547:
                        return new ConstraintViolationException(exInfo.Message, sqle);
                    case 2627:
                        return new UniquenessViolationException(exInfo.Message, sqle);
                    //case 208:
                    //    return new SQLGrammarException(exInfo.Message, sqle.InnerException, exInfo.Sql);
                    //case 3960:
                    //    return new StaleObjectStateException(exInfo.EntityName, exInfo.EntityId);
                }
            }
            return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException, exInfo.Message, exInfo.Sql);
        }
    }
}
