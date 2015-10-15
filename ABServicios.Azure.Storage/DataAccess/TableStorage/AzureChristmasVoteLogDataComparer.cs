using System;
using System.Collections.Generic;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
    public class AzureChristmasVoteLogComparer : IEqualityComparer<QueueMessage<AzureChristmasVoteLog>>
    {
        // Products are equal if their names and log numbers are equal. 
        public bool Equals(QueueMessage<AzureChristmasVoteLog> x, QueueMessage<AzureChristmasVoteLog> y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.Data.UserId == y.Data.UserId && x.Data.Ip == y.Data.Ip && x.Data.Date.Date == y.Data.Date.Date;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(QueueMessage<AzureChristmasVoteLog> log)
        {
            return log.Data.Date.Date.GetHashCode() + log.Data.Ip.GetHashCode();
        }

    }

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

    //public class AzureChristmasReferalDataComparer : IEqualityComparer<QueueMessage<AzureChristmasRefreshReferal>>
    //{
    //    // Products are equal if their names and log numbers are equal. 
    //    public bool Equals(QueueMessage<AzureChristmasRefreshReferal> x, QueueMessage<AzureChristmasRefreshReferal> y)
    //    {
    //
    //        //Check whether the compared objects reference the same data. 
    //        if (Object.ReferenceEquals(x, y)) return true;
    //
    //        //Check whether any of the compared objects is null. 
    //        if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
    //            return false;
    //
    //        //Check whether the products' properties are equal. 
    //        return x.Data.Referal == y.Data.Referal && x.Data.UserId == y.Data.UserId;
    //    }
    //
    //    // If Equals() returns true for a pair of objects  
    //    // then GetHashCode() must return the same value for these objects. 
    //
    //    public int GetHashCode(QueueMessage<AzureChristmasRefreshReferal> log)
    //    {
    //        return log.Data.Referal.GetHashCode() + log.Data.UserId.GetHashCode();
    //    }
    //
    //}
}