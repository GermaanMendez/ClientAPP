
using System.ComponentModel.DataAnnotations;

namespace MVC
{
    public class EmailUsuarioVO
    {
        [EmailAddress]
        [Required]
        public string Valor { get; private set; }

        public EmailUsuarioVO(string val)
        {
            Valor = val;
            //Validar();
        }
        private EmailUsuarioVO()
        {
        }
        public void Validar()
        {

        }

    }

}
