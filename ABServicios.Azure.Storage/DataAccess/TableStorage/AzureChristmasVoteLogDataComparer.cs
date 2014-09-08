using System;
using System.Collections.Generic;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
    public class AzureChristmasVoteLogDataComparer : IEqualityComparer<AzureChristmasVoteLogData>
    {
        // Products are equal if their names and log numbers are equal. 
        public bool Equals(AzureChristmasVoteLogData x, AzureChristmasVoteLogData y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.Ip == y.Ip && x.Date.Date == y.Date.Date;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(AzureChristmasVoteLogData log)
        {
            return log.Date.Date.GetHashCode() + log.Ip.GetHashCode();
        }

    }
}