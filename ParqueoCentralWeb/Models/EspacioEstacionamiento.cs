using System.ComponentModel.DataAnnotations;

namespace ParqueoCentralWeb.Models
{
    public class EspacioEstacionamiento
    {
        [Key]
        public int IdEspacio { get; set; }

        [Required(ErrorMessage = "El código del espacio es obligatorio.")]
        [StringLength(10)]
        [Display(Name = "Código del espacio")]
        public string CodigoEspacio { get; set; }

        [Required(ErrorMessage = "Por favor, seleccione el tipo de espacio.")]
        [Display(Name = "Tipo de espacio")]
        public string TipoEspacio { get; set; }

        [Required]
        public string Estado { get; set; }

        [Display(Name = "Espacio activo")]
        public bool Activo { get; set; }
    }
}