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
    public List<ClienteViewModel> ListarPaginado(
        string? buscar,
        int pagina,
        int tamanoPagina,
        out int totalRegistros)
    {
        var lista = new List<ClienteViewModel>();
        totalRegistros = 0;
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            using (var cmd = new SqlCommand("sp_ListarClientesPaginado", cn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@Buscar",
                    (object?)buscar ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@TamanoPagina", tamanoPagina);

                var paramTotal = new SqlParameter(
                    "@TotalRegistros",
                    System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                cmd.Parameters.Add(paramTotal);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ClienteViewModel
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Nombre = dr["Nombre"].ToString() ?? "",
                            Apellido = dr["Apellido"].ToString() ?? "",
                            Telefono = dr["Telefono"]?.ToString() ?? "",
                            Correo = dr["Correo"]?.ToString()
                        });
                    }
                }

                totalRegistros =
                    paramTotal.Value != DBNull.Value
                        ? Convert.ToInt32(paramTotal.Value)
                        : 0;
            }
        }

        return lista;
    }

    public List<ClienteViewModel> Listar(string? buscar)
    {
        int total;

        return ListarPaginado(
            buscar,
            1,
            1000,
            out total
        );
    }
    public ClienteViewModel? ObtenerPorId(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT
                    IdCliente,
                    Nombre,
                    Apellido,
                    Telefono,
                    Correo
                FROM Cliente
                WHERE IdCliente = @IdCliente";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdCliente", id);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new ClienteViewModel
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Nombre = dr["Nombre"].ToString() ?? "",
                            Apellido = dr["Apellido"].ToString() ?? "",
                            Telefono = dr["Telefono"]?.ToString() ?? "",
                            Correo = dr["Correo"]?.ToString()
                        };
                    }
                }
            }
        }

        return null;
    }

    public void Insertar(ClienteViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                INSERT INTO Cliente
                    (Nombre, Apellido, Telefono, Correo)
                VALUES
                    (@Nombre, @Apellido, @Telefono, @Correo)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@Nombre",
                    modelo.Nombre ?? (object)DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@Apellido",
                    modelo.Apellido ?? (object)DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@Telefono",
                    modelo.Telefono ?? (object)DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@Correo",
                    modelo.Correo ?? (object)DBNull.Value
                );

                cmd.ExecuteNonQuery();
            }
        }
    }
    public bool Actualizar(ClienteViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                UPDATE Cliente
                SET
                    Nombre = @Nombre,
                    Apellido = @Apellido,
                    Telefono = @Telefono,
                    Correo = @Correo
                WHERE IdCliente = @IdCliente";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue(
                    "@IdCliente",
                    modelo.IdCliente
                );

                cmd.Parameters.AddWithValue(
                    "@Nombre",
                    modelo.Nombre ?? (object)DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@Apellido",
                    modelo.Apellido ?? (object)DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@Telefono",
                    modelo.Telefono ?? (object)DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@Correo",
                    modelo.Correo ?? (object)DBNull.Value
                );

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    // DELETE - Eliminar cliente
    public bool Eliminar(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                DELETE FROM Cliente
                WHERE IdCliente = @IdCliente";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdCliente", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}