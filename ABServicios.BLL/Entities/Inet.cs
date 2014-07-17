using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABServicios.BLL.Entities
{
    public class InetForm : AbstractEntity<Guid>
    {
        public InetForm()
        {
            Members = new List<TeamMember>();
        }

        public virtual string Team { get; set; }
        public virtual string Institucion { get; set; }
        public virtual string Provincia { get; set; }

        public virtual Uri FileUrl { get; set; }

        public virtual IEnumerable<TeamMember> Members { get; set; }
        public virtual DateTime Date { get; set; }
    }

    public class TeamMember : AbstractEntity<Guid>
    {
        public virtual string Nombre { get; set; }

        public virtual string Apellido { get; set; }

        public virtual string Email { get; set; }


    }
}
