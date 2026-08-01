using Microsoft.Data.SqlClient;

namespace FulbitoBravo.Data
{
    public class ConexionBD
    {
        private readonly IConfiguration _configuration;
        private readonly string _cadenaSQL;

        public ConexionBD(IConfiguration configuration)
        {
            _configuration = configuration;
            _cadenaSQL = _configuration.GetConnectionString("CadenaSQL")!;
        }

        public string ObtenerCadenaSQL()
        {
            return _cadenaSQL;
        }

        // Método para crear y retornar la conexión
        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_cadenaSQL);
        }
    }
}