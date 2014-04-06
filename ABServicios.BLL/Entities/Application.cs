using System;
using System.Collections.Generic;
using System.Linq;

namespace ABServicios.BLL.Entities
{
	/// <summary>
	/// Aplicaciones registradas para el uso de la API
	/// </summary>
	public class Application: AbstractEquatableEntity<int>
	{
		private string roles;
		/// <summary>
		/// Mnemonico de la empresa registrada
		/// </summary>
		public virtual string Mnemonico { get; set; }

		/// <summary>
		/// Application Key para HMAC
		/// </summary>
		public virtual Guid AppKey { get; set; }

		/// <summary>
		/// Application secret para HMAC
		/// </summary>
		public virtual Guid AppSecret { get; set; }

		/// <summary>
		/// Roles de la applicación para Authorization
		/// </summary>
		public virtual IEnumerable<string> Roles
		{
			get
			{
				return roles == null ? Enumerable.Empty<string>(): from r in roles.Split(',') let rt = r.Trim() where !string.IsNullOrWhiteSpace(rt) select rt;
			}
			set
			{
				roles = value == null ? (string)null: string.Join(",", value.Where(x=> x != null).Select(x=> x.Trim()).Where(x=> !string.IsNullOrWhiteSpace(x)));
			}
		}
	}
}