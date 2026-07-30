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

    public List<PagoViewModel> Listar()
    {
        var lista = new List<PagoViewModel>();
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
            string query = "SELECT IdPago, IdReserva, Monto, FechaPago, EstadoPago FROM Pago";

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

    public void Insertar(PagoViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
            string query = "INSERT INTO Pago (IdReserva, Monto, FechaPago, EstadoPago) VALUES (@IdReserva, @Monto, @FechaPago, @EstadoPago)";

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
}