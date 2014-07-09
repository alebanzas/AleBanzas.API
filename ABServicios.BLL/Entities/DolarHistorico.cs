using System;

namespace ABServicios.BLL.Entities
{
    public class DolarHistorico : AbstractEntity<Guid>
    {
        public virtual DateTime Date { get; set; }
        public virtual double Compra { get; set; }
        public virtual double Venta { get; set; }
        public virtual int Moneda { get; set; }

    }
}
