using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels
{
    public class MantenimientoViewModel
    {
        public int Id { get; set; }
        public DateTime FechaMantenimiento { get; set; }
        public string Descripcion { get; set; }
        public double CostoMantenimiento { get; set; }
        public string NombreEmpleado { get; set; }
        public CabañaViewModel Cabaña { get; set; }
        public int IdCabaña { get; set; }

    }
}
