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

    public List<CanchaViewModel> Listar()
    {
        var lista = new List<CanchaViewModel>();
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
            string query = "SELECT IdCancha, Nombre, Descripcion, Estado FROM Cancha";

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

    public void Insertar(CanchaViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
            string query = "INSERT INTO Cancha (Nombre, Descripcion, Estado) VALUES (@Nombre, @Descripcion, @Estado)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@Nombre", modelo.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object?)modelo.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", modelo.Estado);

                cmd.ExecuteNonQuery();
            }
        }
    }
}