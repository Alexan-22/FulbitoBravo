using FulbitoBravo.Models;

namespace FulbitoBravo.Interfaces
{
    public interface IReservaRepositorio
    {
        Task<bool> RegistrarReservaConPagoAsync(int idCliente, int idCancha, DateTime fechaReserva, int idHorario, decimal monto);
    }
}


