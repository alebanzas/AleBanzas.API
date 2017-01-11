using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;

namespace AB.Data
{
	public class InMemoryApplicationsRepository : IRepository<Application>
	{
		private static readonly ISet<Application> repo;

		static InMemoryApplicationsRepository()
		{
			repo = new HashSet<Application>();
			repo.Add(new Application
			{
				ID = 1,
				Mnemonico = "ConnectionTests",
                AppKey = new Guid("76d82836-b81a-49d0-ab07-b00e202ee001"),
                AppSecret = new Guid("9fd8b6fa-2a68-4a68-b9c7-5a3174daeddc"),
				Roles = new string[0]
			});

            repo.Add(new Application
            {
                ID = 2,
                Mnemonico = "ABServicios",
                AppKey = new Guid("8463adb1-94f4-4436-a046-ac36229f1571"),
                AppSecret = new Guid("68555023-26fe-42f2-b8ff-63a9cbba52c8"),
                Roles = ApplicationsRoles.InHouseApplication
            });

            repo.Add(new Application
            {
                ID = 3,
                Mnemonico = "DolarBlueWP",
                AppKey = new Guid("78f00a47-badb-4b8e-afad-f5e7114d917a"),
                AppSecret = new Guid("7a691d5e-604f-4662-8601-eb6b8c4fca3a"),
                Roles = new[] { ApplicationsRoles.DolarBlue },
            });

            repo.Add(new Application
            {
                ID = 3,
                Mnemonico = "ConsoleApp",
                AppKey = new Guid("09b33d59-0a94-4742-be48-139735636329"),
                AppSecret = new Guid("ae37c9c0-f0f4-41e0-ba2f-c11ac5171166"),
                Roles = new[] { ApplicationsRoles.ConsoleApp },
            });

            repo.Add(new Application
            {
                ID = 3,
                Mnemonico = "Limbs",
                AppKey = new Guid("d973ae0f-cd1c-432c-81e5-b038bca46be9"),
                AppSecret = new Guid("ec5066c7-e133-4b9b-92ac-f9abdc1b582f"),
                Roles = new[] { ApplicationsRoles.Limbs },
            });

        }

		public IEnumerator<Application> GetEnumerator()
		{
			return repo.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Expression Expression
		{
			get { return repo.AsQueryable().Expression; }
		}

		public Type ElementType
		{
			get { return typeof (Application); }
		}

		public IQueryProvider Provider
		{
			get { return repo.AsQueryable().Provider; }
		}

		public Application this[int id]
		{
			get { return repo.SingleOrDefault(x => id == x.ID); }
		}

		public int Count
		{
			get { return repo.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public Application Get(int id)
		{
			return this[id];
		}

		public Application GetProxy(int id)
		{
			return this[id];
		}

		public bool Contains(int id)
		{
			return repo.Any(x => id == x.ID);
		}

		public void Add(Application item)
		{
			throw new NotSupportedException();
		}

		public bool Remove(Application item)
		{
			throw new NotSupportedException();
		}
	}
}