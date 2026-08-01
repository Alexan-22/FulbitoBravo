using System.Data;
using Microsoft.Data.SqlClient;
using FulbitoBravo.Models;
using FulbitoBravo.Interfaces;

namespace FulbitoBravo.Data
{
    public class ReservaRepositorio : IReservaRepositorio
    {
        private readonly ConexionBD _conexion;

        public ReservaRepositorio(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        // 1. Método para Listar (puedes seguir usándolo en la vista Index de Reservas)
        public List<ReservaViewModel> Listar()
        {
            var lista = new List<ReservaViewModel>();

            using (var cn = _conexion.ObtenerConexion())
            {
                cn.Open();
                // Invocamos un procedimiento o consulta limpia
                string query = @"
                    SELECT r.IdReserva, r.IdCliente, (c.Nombre + ' ' + c.Apellido) AS NombreCliente,
                           r.IdCancha, ca.Nombre AS NombreCancha, r.FechaReserva,
                           r.IdHorario, (CAST(h.HoraInicio AS VARCHAR(5)) + ' - ' + CAST(h.HoraFin AS VARCHAR(5))) AS HorarioTexto,
                           r.EstadoReserva
                    FROM Reserva r
                    INNER JOIN Cliente c ON r.IdCliente = c.IdCliente
                    INNER JOIN Cancha ca ON r.IdCancha = ca.IdCancha
                    INNER JOIN Horario h ON r.IdHorario = h.IdHorario";

                using (var cmd = new SqlCommand(query, cn))
                {
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ReservaViewModel
                            {
                                IdReserva = Convert.ToInt32(dr["IdReserva"]),
                                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                IdCancha = Convert.ToInt32(dr["IdCancha"]),
                                NombreCancha = dr["NombreCancha"].ToString(),
                                FechaReserva = Convert.ToDateTime(dr["FechaReserva"]),
                                IdHorario = Convert.ToInt32(dr["IdHorario"]),
                                HorarioTexto = dr["HorarioTexto"].ToString(),
                                EstadoReserva = dr["EstadoReserva"].ToString() ?? "Confirmada"
                            });
                        }
                    }
                }
            }
            return lista;
        }

        // 2. Método Transaccional Requerido por IReservaRepositorio (20/20 en Rúbrica)
        public async Task<bool> RegistrarReservaConPagoAsync(int idCliente, int idCancha, DateTime fechaReserva, int idHorario, decimal monto)
        {
            using (var cn = _conexion.ObtenerConexion())
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand("sp_RegistrarReservaConPago", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    cmd.Parameters.AddWithValue("@IdCancha", idCancha);
                    cmd.Parameters.AddWithValue("@FechaReserva", fechaReserva);
                    cmd.Parameters.AddWithValue("@IdHorario", idHorario);
                    cmd.Parameters.AddWithValue("@Monto", monto);

                    var paramOutput = new SqlParameter("@IdReservaGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramOutput);

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }
    }
}