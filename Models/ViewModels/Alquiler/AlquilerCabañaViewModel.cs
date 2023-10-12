using System.ComponentModel.DataAnnotations.Schema;
using MVC.Models.ViewModels.Cabaña;
using MVC.Models.ViewModels.Usuario;

namespace MVC.Models.ViewModels.Alquiler
{
    public class AlquilerCabañaViewModel
    {
        public int Id { get; set; }
        public DateTime FechaAlquilerDesde { get; set; }
        public DateTime FechaAlquilerHasta { get; set; }
        public int Precio { get; set; }

        public CabañaViewModel Cabaña { get; set; }
        public int CabañaId { get; set; }

        public UsuarioViewModel Usuario { get; set; }
        public int? UsuarioId { get; set; }

        public void AsignarPrecio()
        {
            TimeSpan cantidadDiasAlquiler = FechaAlquilerHasta - FechaAlquilerDesde;
            int diferenciaEnDias = cantidadDiasAlquiler.Days;

            Precio = Cabaña.PrecioDiario * diferenciaEnDias;
        }
    }
}
