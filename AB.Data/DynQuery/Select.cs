using System;

namespace AB.Data.DynQuery
{
	/// <summary>
	/// The class that represent the "select" clause of a HQL/SQL.
	/// </summary>
	/// <remarks>
	/// The syntax is cheked when the HQL/SQL will be parsed.
	/// </remarks>
	[Serializable]
	public class Select : IDynClause
	{
		private readonly string partialClause;
		private readonly bool useDistinct;

		/// <summary>
		/// Create a new instance of <see cref="Select"/>.
		/// </summary>
		/// <param name="partialClause">
		/// The "select" clause, of the query, without the "select" word.
		/// </param>
		/// <exception cref="ArgumentNullException">If <paramref name="partialClause"/> is null or empty.</exception>
		public Select(string partialClause)
			: this(partialClause, false) {}

		private Select(string partialClause, bool useDistinct)
		{
			partialClause = partialClause.Trim();
			if (string.IsNullOrEmpty(partialClause) || partialClause.Length == 0)
				throw new ArgumentNullException("partialClause");
			this.partialClause = partialClause;
			this.useDistinct = useDistinct;
		}

		/// <summary>
		/// Create a new instance of <see cref="Select"/>.
		/// </summary>
		/// <param name="partialClause">
		/// The "select" clause, of the query, without the "select distinct" words.
		/// </param>
		/// <returns>A new instance of <see cref="Select"/>.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="partialClause"/> is null or empty.</exception>
		public static Select Distinct(string partialClause)
		{
			return new Select(partialClause, true);
		}

		private From from;
		public Select From(string partialClause)
		{
			if (from != null)
				throw new NotSupportedException(string.Format("Can't override the 'from' clause; original 'from':{0}", from.Expression));
			from = new From(partialClause);
			return this;
		}

		public From From()
		{
			if (from == null)
				throw new NotSupportedException("'select' without 'from' clause.");
			return from;
		}

		public void SetFrom(From fromClause)
		{
			if (fromClause == null)
				throw new ArgumentNullException("fromClause");

			from = fromClause;
		}

		public Select Where(string partialClause)
		{
			if (from == null)
				throw new NotSupportedException("Can't set the 'where' clause without 'from' clause.");

			from.SetWhere(new Where(partialClause));
			return this;
		}

		#region IDynClause Members

		/// <summary>
		/// The query clause.
		/// </summary>
		public string Clause
		{
			get
			{
				if (from == null || !from.HasMembers)
					throw new NotSupportedException("'select' without 'from' clause.");
				return string.Format("{0} {1} {2}",useDistinct ? "select distinct" : "select",
					Expression, from.Clause);
			}
		}

		#region IQueryPart Members

		/// <summary>
		/// The query part.
		/// </summary>
		public string Expression
		{
			get
			{
				// There's no need to verify the HasMembers, because it can never be empty.
				return partialClause;
			}
		}

		#endregion

		/// <summary>
		/// The clause has some meber or not?
		/// </summary>
		public bool HasMembers
		{
			get { return partialClause.Length > 0; }
		}

		#endregion
	}
}