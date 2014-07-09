using System.Collections.Generic;
using ABServicios.BLL.Entities;

namespace ABServicios.BLL.DataInterfaces.Queries
{
    public interface ISampleQuery
	{
		IList<Hotel> GetCategorias(int tipoCategoria, int paisId);
        Hotel GetCategoria(string categoria, int paisId);
	}
}