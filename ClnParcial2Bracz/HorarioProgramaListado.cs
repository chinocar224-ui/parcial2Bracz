using System;

namespace ClnParcial2Bracz
{
    /// <summary>
    /// Fila para mostrar en la grilla de programas (incluye el nombre del canal).
    /// </summary>
    public class HorarioProgramaListado
    {
        public int id { get; set; }
        public int idCanal { get; set; }
        public string canal { get; set; }
        public string titulo { get; set; }
        public TimeSpan horaInicio { get; set; }
        public TimeSpan horaFin { get; set; }
        public string diaSemana { get; set; }
        public string conductorPrincipal { get; set; }
        public string tipo { get; set; }
    }
}
