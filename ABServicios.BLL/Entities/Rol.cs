using System;

namespace ABServicios.BLL.Entities
{
    [Serializable]
	public class Rol : AbstractEntity<int>
	{
		public virtual string Nombre { get; set; }

		public virtual string Descripcion { get; set; }
	}
}