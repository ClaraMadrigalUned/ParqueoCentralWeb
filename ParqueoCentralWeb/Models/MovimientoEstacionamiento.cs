using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParqueoCentralWeb.Models
{
    public class MovimientoEstacionamiento
    {
        [Key]
        public int IdMovimiento { get; set; }

        [Required]
        [Display(Name = "Vehículo")]
        public int IdVehiculo { get; set; }

        [Required]
        [Display(Name = "Espacio")]
        public int IdEspacio { get; set; }

        [Display(Name = "Fecha y hora de entrada")]
        public DateTime FechaHoraEntrada { get; set; }

        [Display(Name = "Fecha y hora de salida")]
        public DateTime? FechaHoraSalida { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public string EstadoMovimiento { get; set; }

        [Display(Name = "Monto cobrado")]
        public decimal MontoCobrado { get; set; }

        [StringLength(100)]
        [Display(Name = "Usuario que registró")]
        public string UsuarioRegistro { get; set; }

        [ForeignKey("IdVehiculo")]
        public virtual Vehiculo Vehiculo { get; set; }

        [ForeignKey("IdEspacio")]
        public virtual EspacioEstacionamiento EspacioEstacionamiento { get; set; }
    }
}