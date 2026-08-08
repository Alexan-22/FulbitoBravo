using Microsoft.Data.SqlClient;
using FulbitoBravo.Models;

namespace FulbitoBravo.Data;

public class ReservaRepositorio
{
    private readonly ConexionBD _conexion;

    public ReservaRepositorio(ConexionBD conexion)
    {
        _conexion = conexion;
    }

    // Listado simple (sin paginación) 
    public List<ReservaViewModel> Listar()
    {
        int total;
        return ListarPaginado(null, null, 1, 1000, out total);
    }

    // Listado con paginación y filtro por fechas
    public List<ReservaViewModel> ListarPaginado(
        DateTime? fechaInicio, 
        DateTime? fechaFin, 
        int pagina, 
        int tamanoPagina, 
        out int totalRegistros)
    {
        var lista = new List<ReservaViewModel>();
        totalRegistros = 0;
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            using (var cmd = new SqlCommand("sp_ListarReservasReporte", cn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FechaInicio", (object?)fechaInicio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaFin", (object?)fechaFin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@TamanoPagina", tamanoPagina);

                var paramTotal = new SqlParameter("@TotalRegistros", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                cmd.Parameters.Add(paramTotal);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var horaInicio = dr["HoraInicio"] != DBNull.Value 
                            ? ((TimeSpan)dr["HoraInicio"]).ToString(@"hh\:mm") 
                            : "";
                        var horaFin = dr["HoraFin"] != DBNull.Value 
                            ? ((TimeSpan)dr["HoraFin"]).ToString(@"hh\:mm") 
                            : "";

                        lista.Add(new ReservaViewModel
                        {
                            IdReserva = Convert.ToInt32(dr["IdReserva"]),
                            NombreCliente = dr["NombreCliente"]?.ToString(),
                            NombreCancha = dr["NombreCancha"]?.ToString(),
                            FechaReserva = Convert.ToDateTime(dr["FechaReserva"]),
                            HorarioTexto = $"{horaInicio} - {horaFin}",
                            EstadoReserva = dr["EstadoReserva"]?.ToString() ?? "Confirmada",
                            Monto = dr["Monto"] != DBNull.Value ? Convert.ToDecimal(dr["Monto"]) : null,
                            EstadoPago = dr["EstadoPago"]?.ToString()
                        });
                    }
                }

                totalRegistros = paramTotal.Value != DBNull.Value 
                    ? Convert.ToInt32(paramTotal.Value) 
                    : 0;
            }
        }

        return lista;
    }

    public void Insertar(ReservaViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
            string query = @"INSERT INTO Reserva (IdCliente, IdCancha, FechaReserva, IdHorario, EstadoReserva) 
                             VALUES (@IdCliente, @IdCancha, @FechaReserva, @IdHorario, @EstadoReserva)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdCliente", modelo.IdCliente);
                cmd.Parameters.AddWithValue("@IdCancha", modelo.IdCancha);
                cmd.Parameters.AddWithValue("@FechaReserva", modelo.FechaReserva);
                cmd.Parameters.AddWithValue("@IdHorario", modelo.IdHorario);
                cmd.Parameters.AddWithValue("@EstadoReserva", modelo.EstadoReserva ?? "Confirmada");

                cmd.ExecuteNonQuery();
            }
        }
    }
}