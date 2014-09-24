using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public virtual IList<TeamMember> Members { get; set; }
        public virtual DateTime Date { get; set; }

        public virtual string TutorNombre { get; set; }
        public virtual string TutorMail { get; set; }
        public virtual string TutorDireccion { get; set; }
        public virtual string TutorDocumento { get; set; }
    }

    public class TeamMember : AbstractEntity<Guid>
    {
        public virtual string Nombre { get; set; }
        public virtual string Email { get; set; }
        [Display(Name = "DNI")]
        public virtual string Documento { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public virtual DateTime FechaNacimiento { get; set; }
        public virtual string Domicilio { get; set; }
    }
}
