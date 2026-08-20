using Microsoft.Data.SqlClient;
using FulbitoBravo.Models;

namespace FulbitoBravo.Data;

public class CanchaRepositorio
{
    private readonly ConexionBD _conexion;

    public CanchaRepositorio(ConexionBD conexion)
    {
        _conexion = conexion;
    }

    // GET - Listar todas las canchas
    public List<CanchaViewModel> Listar()
    {
        var lista = new List<CanchaViewModel>();
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT IdCancha, Nombre, Descripcion, Estado
                FROM Cancha";

            using (var cmd = new SqlCommand(query, cn))
            {
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CanchaViewModel
                        {
                            IdCancha = Convert.ToInt32(dr["IdCancha"]),
                            Nombre = dr["Nombre"].ToString() ?? "",
                            Descripcion = dr["Descripcion"]?.ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        });
                    }
                }
            }
        }

        return lista;
    }

    // GET - Obtener una cancha por ID
    public CanchaViewModel? ObtenerPorId(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT IdCancha, Nombre, Descripcion, Estado
                FROM Cancha
                WHERE IdCancha = @IdCancha";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdCancha", id);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new CanchaViewModel
                        {
                            IdCancha = Convert.ToInt32(dr["IdCancha"]),
                            Nombre = dr["Nombre"].ToString() ?? "",
                            Descripcion = dr["Descripcion"]?.ToString(),
                            Estado = Convert.ToBoolean(dr["Estado"])
                        };
                    }
                }
            }
        }

        return null;
    }

    // POST - Insertar una cancha
    public void Insertar(CanchaViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                INSERT INTO Cancha
                    (Nombre, Descripcion, Estado)
                VALUES
                    (@Nombre, @Descripcion, @Estado)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@Nombre", modelo.Nombre);
                cmd.Parameters.AddWithValue(
                    "@Descripcion",
                    (object?)modelo.Descripcion ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue("@Estado", modelo.Estado);

                cmd.ExecuteNonQuery();
            }
        }
    }

    // PUT - Actualizar una cancha
    public bool Actualizar(CanchaViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                UPDATE Cancha
                SET Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Estado = @Estado
                WHERE IdCancha = @IdCancha";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdCancha", modelo.IdCancha);
                cmd.Parameters.AddWithValue("@Nombre", modelo.Nombre);
                cmd.Parameters.AddWithValue(
                    "@Descripcion",
                    (object?)modelo.Descripcion ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue("@Estado", modelo.Estado);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    // DELETE - Eliminar una cancha
    public bool Eliminar(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                DELETE FROM Cancha
                WHERE IdCancha = @IdCancha";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdCancha", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}