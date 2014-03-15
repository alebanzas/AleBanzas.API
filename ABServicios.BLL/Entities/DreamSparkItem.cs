using System;

namespace ABServicios.BLL.Entities
{
    public class DreamSparkItem : AbstractEntity<Guid>
    {
        public virtual string Nombre { get; set; }

        public virtual string Apellido { get; set; }

        public virtual string Email { get; set; }

        public virtual string Codigo { get; set; }

    }
}
