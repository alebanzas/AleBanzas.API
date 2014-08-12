using System;

namespace ABServicios.BLL.Entities
{
    public class CandyApp : AbstractEntity<Guid>
    {
        public virtual string Nombre { get; set; }
        public virtual string Imagen { get; set; }
        public virtual string Url { get; set; }
    }
}
