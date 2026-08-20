using Microsoft.Data.SqlClient;
using FulbitoBravo.Models;

namespace FulbitoBravo.Data;

public class HorarioRepositorio
{
    private readonly ConexionBD _conexion;

    public HorarioRepositorio(ConexionBD conexion)
    {
        _conexion = conexion;
    }
    public List<HorarioViewModel> Listar()
    {
        var lista = new List<HorarioViewModel>();
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT
                    IdHorario,
                    HoraInicio,
                    HoraFin
                FROM Horario
                ORDER BY HoraInicio";

            using (var cmd = new SqlCommand(query, cn))
            {
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new HorarioViewModel
                        {
                            IdHorario = Convert.ToInt32(dr["IdHorario"]),
                            HoraInicio = (TimeSpan)dr["HoraInicio"],
                            HoraFin = (TimeSpan)dr["HoraFin"]
                        });
                    }
                }
            }
        }

        return lista;
    }

    public HorarioViewModel? ObtenerPorId(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT
                    IdHorario,
                    HoraInicio,
                    HoraFin
                FROM Horario
                WHERE IdHorario = @IdHorario";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdHorario", id);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new HorarioViewModel
                        {
                            IdHorario = Convert.ToInt32(dr["IdHorario"]),
                            HoraInicio = (TimeSpan)dr["HoraInicio"],
                            HoraFin = (TimeSpan)dr["HoraFin"]
                        };
                    }
                }
            }
        }

        return null;
    }

    // POST - Insertar horario
    public void Insertar(HorarioViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                INSERT INTO Horario
                    (HoraInicio, HoraFin)
                VALUES
                    (@HoraInicio, @HoraFin)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@HoraInicio",
                    modelo.HoraInicio
                );

                cmd.Parameters.AddWithValue(
                    "@HoraFin",
                    modelo.HoraFin
                );

                cmd.ExecuteNonQuery();
            }
        }
    }
    public bool Actualizar(HorarioViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                UPDATE Horario
                SET
                    HoraInicio = @HoraInicio,
                    HoraFin = @HoraFin
                WHERE IdHorario = @IdHorario";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@IdHorario",
                    modelo.IdHorario
                );

                cmd.Parameters.AddWithValue(
                    "@HoraInicio",
                    modelo.HoraInicio
                );

                cmd.Parameters.AddWithValue(
                    "@HoraFin",
                    modelo.HoraFin
                );

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    // DELETE - Eliminar horario
    public bool Eliminar(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                DELETE FROM Horario
                WHERE IdHorario = @IdHorario";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@IdHorario",
                    id
                );

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}