using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels
{
    public class TipoCabañaViewModel
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-ZñÑ]+([a-zA-ZñÑ ]*[a-zA-ZñÑ])?$", ErrorMessage = "El campo Nombre debe contener solamente caracteres alfabéticos y espacios embebidos, pero no al principio o al final.")]
        [Column(TypeName = "nvarchar(100)")]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

    }
}
