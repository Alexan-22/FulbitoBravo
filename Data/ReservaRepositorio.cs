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
        var lista = new List<ReservaViewModel>();
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
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
                cmd.Parameters.AddWithValue("@EstadoReserva", modelo.EstadoReserva);

                cmd.ExecuteNonQuery();
            }
        }
    }
}