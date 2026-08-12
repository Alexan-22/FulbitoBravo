using Microsoft.Data.SqlClient;
using FulbitoBravo.Models;

namespace FulbitoBravo.Data;

public class PagoRepositorio
{
    private readonly ConexionBD _conexion;

    public PagoRepositorio(ConexionBD conexion)
    {
        _conexion = conexion;
    }

    // GET - Listar todos los pagos
    public List<PagoViewModel> Listar()
    {
        var lista = new List<PagoViewModel>();
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT
                    IdPago,
                    IdReserva,
                    Monto,
                    FechaPago,
                    EstadoPago
                FROM Pago";

            using (var cmd = new SqlCommand(query, cn))
            {
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new PagoViewModel
                        {
                            IdPago = Convert.ToInt32(dr["IdPago"]),
                            IdReserva = Convert.ToInt32(dr["IdReserva"]),
                            Monto = Convert.ToDecimal(dr["Monto"]),
                            FechaPago = Convert.ToDateTime(dr["FechaPago"]),
                            EstadoPago = dr["EstadoPago"].ToString() ?? "Pagado"
                        });
                    }
                }
            }
        }

        return lista;
    }

    // GET - Obtener pago por ID
    public PagoViewModel? ObtenerPorId(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                SELECT
                    IdPago,
                    IdReserva,
                    Monto,
                    FechaPago,
                    EstadoPago
                FROM Pago
                WHERE IdPago = @IdPago";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdPago", id);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new PagoViewModel
                        {
                            IdPago = Convert.ToInt32(dr["IdPago"]),
                            IdReserva = Convert.ToInt32(dr["IdReserva"]),
                            Monto = Convert.ToDecimal(dr["Monto"]),
                            FechaPago = Convert.ToDateTime(dr["FechaPago"]),
                            EstadoPago = dr["EstadoPago"].ToString() ?? "Pagado"
                        };
                    }
                }
            }
        }

        return null;
    }

    // POST - Insertar pago
    public void Insertar(PagoViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                INSERT INTO Pago
                    (IdReserva, Monto, FechaPago, EstadoPago)
                VALUES
                    (@IdReserva, @Monto, @FechaPago, @EstadoPago)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdReserva", modelo.IdReserva);
                cmd.Parameters.AddWithValue("@Monto", modelo.Monto);
                cmd.Parameters.AddWithValue("@FechaPago", modelo.FechaPago);
                cmd.Parameters.AddWithValue("@EstadoPago", modelo.EstadoPago);

                cmd.ExecuteNonQuery();
            }
        }
    }

    // PUT - Actualizar pago
    public bool Actualizar(PagoViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                UPDATE Pago
                SET
                    IdReserva = @IdReserva,
                    Monto = @Monto,
                    FechaPago = @FechaPago,
                    EstadoPago = @EstadoPago
                WHERE IdPago = @IdPago";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdPago", modelo.IdPago);
                cmd.Parameters.AddWithValue("@IdReserva", modelo.IdReserva);
                cmd.Parameters.AddWithValue("@Monto", modelo.Monto);
                cmd.Parameters.AddWithValue("@FechaPago", modelo.FechaPago);
                cmd.Parameters.AddWithValue("@EstadoPago", modelo.EstadoPago);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    // DELETE - Eliminar pago
    public bool Eliminar(int id)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();

            string query = @"
                DELETE FROM Pago
                WHERE IdPago = @IdPago";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@IdPago", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}