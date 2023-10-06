using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models.ViewModels
{
    public class AlquilerCabañaViewModel
    {
        public DateTime FechaAlquilerDesde { get; set; }
        public DateTime FechaAlquilerHasta { get; set; }
        public int Precio { get; set; }

        public CabañaViewModel Cabaña { get; set; }
        public int CabañaId { get; set; }

        public UsuarioLoginViewModel Usuario { get; set; }
        public int UsuarioId { get; set; }

        public void AsignarPrecio()
        {
            TimeSpan cantidadDiasAlquiler = FechaAlquilerHasta - FechaAlquilerDesde;
            int diferenciaEnDias = cantidadDiasAlquiler.Days;

            this.Precio = Cabaña.PrecioDiario * diferenciaEnDias;
        }
    }
}
