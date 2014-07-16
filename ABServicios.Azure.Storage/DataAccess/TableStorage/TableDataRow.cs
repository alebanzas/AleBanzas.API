using System;
using System.Data.Services.Common;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
    [DataServiceKey(new[] { "PartitionKey", "RowKey" })]
	[CLSCompliant(false)]
	public abstract class TableDataRow
	{
		private string partitionKey;
		private int? requestedHashCode;
		private string rowKey;

		public string PartitionKey
		{
			get { return partitionKey ?? (partitionKey = CreatePartitionKey()); }
			set { partitionKey = value; }
		}

		public string RowKey
		{
			get { return rowKey ?? (rowKey = CreateRowKey()); }
			set { rowKey = value; }
		}

		public DateTime Timestamp { get; set; }

		protected abstract string CreatePartitionKey();
		protected abstract string CreateRowKey();

		public override bool Equals(object obj)
		{
			return Equals(obj as TableDataRow);
		}

		public bool Equals(TableDataRow other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return GetType().IsAssignableFrom(other.GetType()) && Equals(other.PartitionKey, PartitionKey) &&
			       Equals(other.RowKey, RowKey);
		}

		public override int GetHashCode()
		{
			if (!requestedHashCode.HasValue)
			{
				unchecked
				{
					requestedHashCode = (GetType().GetHashCode()*397) ^ (PartitionKey != null ? PartitionKey.GetHashCode() : 0) ^
					                    (RowKey != null ? RowKey.GetHashCode() : 0);
				}
			}
			return requestedHashCode.Value;
		}
	}
}