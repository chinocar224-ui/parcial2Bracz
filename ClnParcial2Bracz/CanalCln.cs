using CadParcial2Bracz;
using System.Collections.Generic;
using System.Linq;

namespace ClnParcial2Bracz
{
    public class CanalCln
    {
        public static List<Canal> listar()
        {
            using (var context = new Parcial2BraczEntities())
            {
                short activo = (short)Estado.Activo;
                return context.Canal
                    .Where(x => x.estado == activo)
                    .OrderBy(x => x.nombre)
                    .ToList();
            }
        }
    }
}
