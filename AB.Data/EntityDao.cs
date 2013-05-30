using ABServicios.BLL.DataInterfaces;
using NHibernate;

namespace AB.Data
{
	public class EntityDao<T, TId> : IEntityDao<T, TId>
	{
		public EntityDao(ISessionFactory factory)
		{
			Factory = factory;
		}
		public ISessionFactory Factory { get; private set; }

		#region Implementation of IDao<T,TId>

		public T GetById(TId id)
		{
			return Session.Get<T>(id);
		}

		protected ISession Session
		{
			get { return Factory.GetCurrentSession(); }
		}

		public T GetProxy(TId id)
		{
			return Session.Load<T>(id);
		}

		public T GetWithLock(TId id)
		{
			return Session.Get<T>(id, LockMode.Upgrade);
		}

		#endregion

		#region Implementation of ICrudDao<T,TId>

		public T Save(T entity)
		{
			Session.Save(entity);
			return entity;
		}

		public T Update(T entity)
		{
			Session.Update(entity);
			return entity;
		}

		public T SaveOrUpdate(T entity)
		{
			Session.SaveOrUpdate(entity);
			return entity;
		}

		public void Delete(T entity)
		{
			Session.Delete(entity);
		}

		#endregion
	}
}