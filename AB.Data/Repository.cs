using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ABServicios.BLL.DataInterfaces;
using NHibernate;
using NHibernate.Linq;

namespace AB.Data
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity: class 
	{
		private readonly ISessionFactory sessionFactory;

		public Repository(ISessionFactory sessionFactory)
		{
			this.sessionFactory = sessionFactory;
		}

		protected ISession Session
		{
			get { return sessionFactory.GetCurrentSession(); }
		}

		public virtual void Add(TEntity item)
		{
			Session.Persist(item);
		}

		public virtual bool Remove(TEntity item)
		{
			Session.Delete(item);
			return true;
		}

		public virtual int Count
		{
			get { return Session.Query<TEntity>().Count(); }
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		#region Implementation of IQueryable

		public virtual Expression Expression
		{
			get { return Session.Query<TEntity>().Expression; }
		}

		public virtual Type ElementType
		{
			get { return Session.Query<TEntity>().ElementType; }
		}

		public virtual IQueryProvider Provider
		{
			get { return Session.Query<TEntity>().Provider; }
		}

		#endregion

		#region Implementation of IEnumerable

		public virtual IEnumerator<TEntity> GetEnumerator()
		{
			return Session.Query<TEntity>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of IRepository<TEntity>

		public virtual TEntity this[int id]
		{
			get { return Session.Get<TEntity>(id); }
		}

		public virtual TEntity Get(int id)
		{
			return Session.Get<TEntity>(id);
		}

		public virtual TEntity GetProxy(int id)
		{
			return Session.Load<TEntity>(id);
		}

		public virtual bool Contains(int id)
		{
			return Get(id) != null;
		}

		#endregion
	}
}