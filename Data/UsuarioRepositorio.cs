using Microsoft.Data.SqlClient;
using FulbitoBravo.Models;

namespace FulbitoBravo.Data;

public class UsuarioRepositorio
{
    private readonly ConexionBD _conexion;

    public UsuarioRepositorio(ConexionBD conexion)
    {
        _conexion = conexion;
    }

    public UsuarioViewModel? ObtenerPorUsername(string username)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT IdUsuario, Username, PasswordHash, Rol, IdCliente, Activo
                FROM Usuario
                WHERE Username = @Username";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new UsuarioViewModel
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            Username = dr["Username"].ToString() ?? "",
                            PasswordHash = dr["PasswordHash"].ToString() ?? "",
                            Rol = dr["Rol"].ToString() ?? "Cliente",
                            IdCliente = dr["IdCliente"] != DBNull.Value ? Convert.ToInt32(dr["IdCliente"]) : null,
                            Activo = Convert.ToBoolean(dr["Activo"])
                        };
                    }
                }
            }
        }

        return null;
    }

    public bool ExisteUsername(string username)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            using (var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Usuario WHERE Username = @Username", cn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }

    public bool ExisteAdmin()
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            using (var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Usuario WHERE Rol = 'Admin'", cn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }

    public int Crear(UsuarioViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                INSERT INTO Usuario (Username, PasswordHash, Rol, IdCliente, Activo)
                OUTPUT INSERTED.IdUsuario
                VALUES (@Username, @PasswordHash, @Rol, @IdCliente, @Activo)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@Username", modelo.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", modelo.PasswordHash);
                cmd.Parameters.AddWithValue("@Rol", modelo.Rol);
                cmd.Parameters.AddWithValue("@IdCliente", (object?)modelo.IdCliente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Activo", modelo.Activo);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
