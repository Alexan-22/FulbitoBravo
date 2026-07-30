using Microsoft.Data.SqlClient;
using FulbitoBravo.Models;

namespace FulbitoBravo.Data;

public class ClienteRepositorio
{
    private readonly ConexionBD _conexion;

    public ClienteRepositorio(ConexionBD conexion)
    {
        _conexion = conexion;
    }

    public List<ClienteViewModel> Listar(string? buscar)
    {
        var lista = new List<ClienteViewModel>();
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
            string query = "SELECT IdCliente, Nombre, Apellido, Telefono, Correo FROM Cliente";

            if (!string.IsNullOrEmpty(buscar))
            {
                query += " WHERE Nombre LIKE @Buscar OR Apellido LIKE @Buscar";
            }

            using (var cmd = new SqlCommand(query, cn))
            {
                if (!string.IsNullOrEmpty(buscar))
                {
                    cmd.Parameters.AddWithValue("@Buscar", "%" + buscar + "%");
                }

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ClienteViewModel
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Nombre = dr["Nombre"].ToString() ?? "",
                            Apellido = dr["Apellido"].ToString() ?? "",
                            Telefono = dr["Telefono"]?.ToString(),
                            Correo = dr["Correo"]?.ToString()
                        });
                    }
                }
            }
        }
        return lista;
    }

    public void Insertar(ClienteViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
            string query = "INSERT INTO Cliente (Nombre, Apellido, Telefono, Correo) VALUES (@Nombre, @Apellido, @Telefono, @Correo)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@Nombre", modelo.Nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Apellido", modelo.Apellido ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", modelo.Telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", modelo.Correo ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}