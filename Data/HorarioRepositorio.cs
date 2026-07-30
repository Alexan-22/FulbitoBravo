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
            string query = "SELECT IdHorario, HoraInicio, HoraFin FROM Horario";

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

    public void Insertar(HorarioViewModel modelo)
    {
        var cadena = _conexion.ObtenerCadenaSQL();

        using (var cn = new SqlConnection(cadena))
        {
            cn.Open();
            string query = "INSERT INTO Horario (HoraInicio, HoraFin) VALUES (@HoraInicio, @HoraFin)";

            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@HoraInicio", modelo.HoraInicio);
                cmd.Parameters.AddWithValue("@HoraFin", modelo.HoraFin);

                cmd.ExecuteNonQuery();
            }
        }
    }
}