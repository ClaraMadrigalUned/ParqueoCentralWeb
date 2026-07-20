using System.ComponentModel.DataAnnotations;

namespace ParqueoCentralWeb.Models
{
    public class Vehiculo
    {
        [Key]
        public int IdVehiculo { get; set; }

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(15, ErrorMessage = "La placa no puede superar los 15 caracteres.")]
        [Display(Name = "Placa")]
        public string Placa { get; set; }

        [Required(ErrorMessage = "Por favor, seleccione el tipo de vehículo.")]
        [Display(Name = "Tipo de vehículo")]
        public string TipoVehiculo { get; set; }

        [Required(ErrorMessage = "Por favor ingrese el nombre del propietario o conductor del vehículo.")]
        [StringLength(100)]
        [Display(Name = "Propietario o conductor")]
        public string Propietario { get; set; }

        [StringLength(20)]
        [Display(Name = "Número de contacto")]
        public string Contacto { get; set; }
    }
}