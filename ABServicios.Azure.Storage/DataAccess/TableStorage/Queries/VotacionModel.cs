using System.Collections.Generic;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage.Queries
{
    public class VotacionModel
    {
        public VotacionModel()
        {
            Lista = new List<VotacionItem>();
        }
        public List<VotacionItem> Lista { get; set; }
    }
    public class VotacionItem
    {
        public string Nombre { get; set; }
        public int Count { get; set; }
    }
}