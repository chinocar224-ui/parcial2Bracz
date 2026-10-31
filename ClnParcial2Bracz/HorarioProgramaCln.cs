using CadParcial2Bracz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnParcial2Bracz
{
    public class HorarioProgramaCln
    {
        public static int crear(HorarioPrograma programa)
        {
            using (var context = new Parcial2BraczEntities())
            {
                validar(context, programa);

                programa.estado = (short)Estado.Activo;
                context.HorarioPrograma.Add(programa);
                context.SaveChanges();
                return programa.id;
            }
        }

        public static int actualizar(HorarioPrograma programa)
        {
            using (var context = new Parcial2BraczEntities())
            {
                var existe = context.HorarioPrograma.Find(programa.id);
                if (existe == null) throw new Exception($"El programa con id {programa.id} no existe");

                validar(context, programa);

                existe.idCanal = programa.idCanal;
                existe.titulo = programa.titulo;
                existe.horaInicio = programa.horaInicio;
                existe.horaFin = programa.horaFin;
                existe.diaSemana = programa.diaSemana;
                existe.tipo = programa.tipo;
                existe.conductorPrincipal = programa.conductorPrincipal;

                return context.SaveChanges();
            }
        }

        /// <summary>
        /// Eliminación lógica: no se borra el registro, sólo se cambia su estado a -1.
        /// </summary>
        public static int eliminar(int id)
        {
            using (var context = new Parcial2BraczEntities())
            {
                var existe = context.HorarioPrograma.Find(id);
                if (existe == null) throw new Exception($"El programa con id {id} no existe");

                existe.estado = (short)Estado.Eliminado;
                return context.SaveChanges();
            }
        }

        public static HorarioPrograma obtenerUno(int id)
        {
            using (var context = new Parcial2BraczEntities())
            {
                var programa = context.HorarioPrograma.Find(id);
                if (programa == null) throw new Exception($"El programa con id {id} no existe");
                return programa;
            }
        }

        public static List<HorarioProgramaListado> listar(string parametro)
        {
            parametro = (parametro ?? string.Empty).Trim();

            using (var context = new Parcial2BraczEntities())
            {
                short activo = (short)Estado.Activo;
                var programas = context.HorarioPrograma
                    .Where(x => x.estado == activo &&
                        (x.titulo.Contains(parametro) ||
                         x.conductorPrincipal.Contains(parametro) ||
                         x.diaSemana.Contains(parametro) ||
                         x.Canal.nombre.Contains(parametro)))
                    .Select(x => new HorarioProgramaListado
                    {
                        id = x.id,
                        idCanal = x.idCanal,
                        canal = x.Canal.nombre,
                        titulo = x.titulo,
                        horaInicio = x.horaInicio,
                        horaFin = x.horaFin,
                        diaSemana = x.diaSemana,
                        tipo = x.tipo,
                        conductorPrincipal = x.conductorPrincipal
                    })
                    .ToList();

                return programas
                    .OrderBy(x => x.canal)
                    .ThenBy(x => Array.IndexOf(Constantes.DiasSemana, x.diaSemana))
                    .ThenBy(x => x.horaInicio)
                    .ToList();
            }
        }

        private static void validar(Parcial2BraczEntities context, HorarioPrograma programa)
        {
            if (!Constantes.Tipos.Contains(programa.tipo))
                throw new Exception($"El tipo '{programa.tipo}' no es válido");

            if (!Constantes.DiasSemana.Contains(programa.diaSemana))
                throw new Exception($"El día de la semana '{programa.diaSemana}' no es válido");

            if (programa.horaFin <= programa.horaInicio)
                throw new Exception("La hora de fin debe ser mayor a la hora de inicio");

            short activo = (short)Estado.Activo;
            int idPrograma = programa.id;
            int idCanal = programa.idCanal;
            string dia = programa.diaSemana;
            TimeSpan inicio = programa.horaInicio;
            TimeSpan fin = programa.horaFin;

            bool seSuperpone = context.HorarioPrograma.Any(x =>
                x.estado == activo &&
                x.id != idPrograma &&
                x.idCanal == idCanal &&
                x.diaSemana == dia &&
                inicio < x.horaFin &&
                fin > x.horaInicio);

            if (seSuperpone)
                throw new Exception("El horario se superpone con otro programa del mismo canal y día");
        }
    }
}
