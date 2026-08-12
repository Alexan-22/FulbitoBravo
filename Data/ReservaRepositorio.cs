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

    public List<ReservaViewModel> Listar()
    {
        int total;

        return ListarPaginado(
            null,
            null,
            1,
            1000,
            out total
        );
    }

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

            using (var cmd = new SqlCommand(
                "sp_ListarReservasReporte",
                cn))
            {
                cmd.CommandType =
                    System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@FechaInicio",
                    (object?)fechaInicio ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@FechaFin",
                    (object?)fechaFin ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@Pagina",
                    pagina
                );

                cmd.Parameters.AddWithValue(
                    "@TamanoPagina",
                    tamanoPagina
                );

                var paramTotal = new SqlParameter(
                    "@TotalRegistros",
                    System.Data.SqlDbType.Int)
                {
                    Direction =
                        System.Data.ParameterDirection.Output
                };

                cmd.Parameters.Add(paramTotal);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var horaInicio =
                            dr["HoraInicio"] != DBNull.Value
                                ? ((TimeSpan)dr["HoraInicio"])
                                    .ToString(@"hh\:mm")
                                : "";

                        var horaFin =
                            dr["HoraFin"] != DBNull.Value
                                ? ((TimeSpan)dr["HoraFin"])
                                    .ToString(@"hh\:mm")
                                : "";
                        lista.Add(new ReservaViewModel { 
                            IdReserva = Convert.ToInt32(dr["IdReserva"]), 
                            IdCliente = Convert.ToInt32(dr["IdCliente"]), 
                            NombreCliente = dr["NombreCliente"]?.ToString(), 
                            IdCancha = Convert.ToInt32(dr["IdCancha"]), 
                            NombreCancha = dr["NombreCancha"]?.ToString(), 
                            FechaReserva = Convert.ToDateTime(dr["FechaReserva"]), 
                            IdHorario = Convert.ToInt32(dr["IdHorario"]), 
                            HorarioTexto = $"{horaInicio} - {horaFin}", 
                            EstadoReserva = dr["EstadoReserva"]?.ToString() ?? "Confirmada", 
                            Monto = dr["Monto"] != DBNull.Value ? Convert.ToDecimal(dr["Monto"]) : null, 
                            EstadoPago = dr["EstadoPago"]?.ToString() 
                        });
                     }
                }

                totalRegistros =
                    paramTotal.Value != DBNull.Value
                        ? Convert.ToInt32(
                            paramTotal.Value)
                        : 0;
            }
        }

        return lista;
    }

    public ReservaViewModel? ObtenerPorId(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT
                    r.IdReserva,
                    r.IdCliente,
                    c.Nombre + ' ' + c.Apellido AS NombreCliente,
                    r.IdCancha,
                    ca.Nombre AS NombreCancha,
                    r.FechaReserva,
                    r.IdHorario,
                    h.HoraInicio,
                    h.HoraFin,
                    r.EstadoReserva
                FROM Reserva r
                INNER JOIN Cliente c
                    ON r.IdCliente = c.IdCliente
                INNER JOIN Cancha ca
                    ON r.IdCancha = ca.IdCancha
                INNER JOIN Horario h
                    ON r.IdHorario = h.IdHorario
                WHERE r.IdReserva = @IdReserva";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@IdReserva",
                    id
                );

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        var horaInicio =
                            dr["HoraInicio"] != DBNull.Value
                                ? ((TimeSpan)dr["HoraInicio"])
                                    .ToString(@"hh\:mm")
                                : "";

                        var horaFin =
                            dr["HoraFin"] != DBNull.Value
                                ? ((TimeSpan)dr["HoraFin"])
                                    .ToString(@"hh\:mm")
                                : "";

                        return new ReservaViewModel
                        {
                            IdReserva =
                                Convert.ToInt32(
                                    dr["IdReserva"]),

                            IdCliente =
                                Convert.ToInt32(
                                    dr["IdCliente"]),

                            NombreCliente =
                                dr["NombreCliente"]?.ToString(),

                            IdCancha =
                                Convert.ToInt32(
                                    dr["IdCancha"]),

                            NombreCancha =
                                dr["NombreCancha"]?.ToString(),

                            FechaReserva =
                                Convert.ToDateTime(
                                    dr["FechaReserva"]),

                            IdHorario =
                                Convert.ToInt32(
                                    dr["IdHorario"]),

                            HorarioTexto =
                                $"{horaInicio} - {horaFin}",

                            EstadoReserva =
                                dr["EstadoReserva"]?.ToString()
                                ?? "Confirmada"
                        };
                    }
                }
            }
        }

        return null;
    }

    public void Insertar(ReservaViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                INSERT INTO Reserva
                    (
                        IdCliente,
                        IdCancha,
                        FechaReserva,
                        IdHorario,
                        EstadoReserva
                    )
                VALUES
                    (
                        @IdCliente,
                        @IdCancha,
                        @FechaReserva,
                        @IdHorario,
                        @EstadoReserva
                    )";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@IdCliente",
                    modelo.IdCliente
                );

                cmd.Parameters.AddWithValue(
                    "@IdCancha",
                    modelo.IdCancha
                );

                cmd.Parameters.AddWithValue(
                    "@FechaReserva",
                    modelo.FechaReserva
                );

                cmd.Parameters.AddWithValue(
                    "@IdHorario",
                    modelo.IdHorario
                );

                cmd.Parameters.AddWithValue(
                    "@EstadoReserva",
                    modelo.EstadoReserva ?? "Confirmada"
                );

                cmd.ExecuteNonQuery();
            }
        }
    }

    public bool Actualizar(ReservaViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                UPDATE Reserva
                SET
                    IdCliente = @IdCliente,
                    IdCancha = @IdCancha,
                    FechaReserva = @FechaReserva,
                    IdHorario = @IdHorario,
                    EstadoReserva = @EstadoReserva
                WHERE IdReserva = @IdReserva";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@IdReserva",
                    modelo.IdReserva
                );

                cmd.Parameters.AddWithValue(
                    "@IdCliente",
                    modelo.IdCliente
                );

                cmd.Parameters.AddWithValue(
                    "@IdCancha",
                    modelo.IdCancha
                );

                cmd.Parameters.AddWithValue(
                    "@FechaReserva",
                    modelo.FechaReserva
                );

                cmd.Parameters.AddWithValue(
                    "@IdHorario",
                    modelo.IdHorario
                );

                cmd.Parameters.AddWithValue(
                    "@EstadoReserva",
                    modelo.EstadoReserva ?? "Confirmada"
                );

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public bool Eliminar(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                DELETE FROM Reserva
                WHERE IdReserva = @IdReserva";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@IdReserva",
                    id
                );

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}