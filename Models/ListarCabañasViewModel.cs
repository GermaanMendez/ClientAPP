

namespace MVC.Models
{
    public class ListarCabañasViewModel
    {
            public int NumeroHabitacion { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public bool TieneJacuzzi { get; set; }
            public bool EstaHabilitada { get; set; }
            public string Foto { get; set; }
            public string NombreTipoCabaña { get; set; }
            public decimal CostoPorPersona { get; set; }

    }
}
