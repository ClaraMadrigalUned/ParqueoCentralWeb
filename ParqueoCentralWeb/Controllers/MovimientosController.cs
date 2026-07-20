using ParqueoCentralWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ParqueoCentralWeb.Controllers
{
    public class MovimientosController : Controller
    {
        private ParqueoContext db = new ParqueoContext();

        // Función para mostrar la lista de movimientos de estacionamiento
        public ActionResult Index()
        {
            var movimientos = db.Movimientos
                .Include(m => m.Vehiculo)
                .Include(m => m.EspacioEstacionamiento)
                .OrderByDescending(m => m.FechaHoraEntrada)
                .ToList();

            return View(movimientos);
        }

        // Función para mostrar el formulario de registro de entrada
        public ActionResult Create()
        {
            CargarListas();

            return View();
        }

        // Función para registrar la entrada de un vehículo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MovimientoEstacionamiento movimiento)
        {
            MovimientoEstacionamiento entradaActiva =
                db.Movimientos.FirstOrDefault(m =>
                    m.IdVehiculo == movimiento.IdVehiculo &&
                    m.EstadoMovimiento == "Activo");

            if (entradaActiva != null)
            {
                ModelState.AddModelError(
                    "IdVehiculo",
                    "Este vehículo ya tiene una entrada activa."
                );
            }

            EspacioEstacionamiento espacio =
                db.Espacios.Find(movimiento.IdEspacio);

            if (espacio == null)
            {
                ModelState.AddModelError(
                    "IdEspacio",
                    "El espacio seleccionado no existe."
                );
            }
            else if (!espacio.Activo)
            {
                ModelState.AddModelError(
                    "IdEspacio",
                    "El espacio seleccionado está inactivo."
                );
            }
            else if (espacio.Estado != "Disponible")
            {
                ModelState.AddModelError(
                    "IdEspacio",
                    "El espacio seleccionado ya está ocupado."
                );
            }

            movimiento.FechaHoraEntrada = DateTime.Now;
            movimiento.FechaHoraSalida = null;
            movimiento.EstadoMovimiento = "Activo";
            movimiento.MontoCobrado = 0;
            movimiento.UsuarioRegistro =
            Session["NombreOperador"]?.ToString() ?? "Operador";

            ModelState.Remove("FechaHoraEntrada");
            ModelState.Remove("FechaHoraSalida");
            ModelState.Remove("EstadoMovimiento");
            ModelState.Remove("MontoCobrado");
            ModelState.Remove("UsuarioRegistro");

            if (ModelState.IsValid)
            {
                espacio.Estado = "Ocupado";

                db.Movimientos.Add(movimiento);
                db.SaveChanges();

                TempData["MensajeExito"] =
                    "La entrada del vehículo se registró correctamente.";

                return RedirectToAction("Index");
            }

            CargarListas(movimiento.IdVehiculo, movimiento.IdEspacio);

            return View(movimiento);
        }

        // Función para mostrar la información antes de registrar la salida
        public ActionResult Salida(int? id)
        {
            if (id == null)
            {
                TempData["MensajeError"] =
                    "Debe seleccionar un movimiento.";

                return RedirectToAction("Index");
            }

            MovimientoEstacionamiento movimiento =
                db.Movimientos
                    .Include(m => m.Vehiculo)
                    .Include(m => m.EspacioEstacionamiento)
                    .FirstOrDefault(m => m.IdMovimiento == id);

            if (movimiento == null)
            {
                TempData["MensajeError"] =
                    "El movimiento seleccionado no existe.";

                return RedirectToAction("Index");
            }

            if (movimiento.EstadoMovimiento != "Activo")
            {
                TempData["MensajeError"] =
                    "Este movimiento ya fue finalizado.";

                return RedirectToAction("Index");
            }

            return View(movimiento);
        }

        // Función para registrar la salida y calcular el monto a pagar.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalidaConfirmada(int id)
        {
            MovimientoEstacionamiento movimiento =
                db.Movimientos
                    .Include(m => m.EspacioEstacionamiento)
                    .FirstOrDefault(m => m.IdMovimiento == id);

            if (movimiento == null)
            {
                TempData["MensajeError"] =
                    "El movimiento seleccionado no existe.";

                return RedirectToAction("Index");
            }

            if (movimiento.EstadoMovimiento != "Activo")
            {
                TempData["MensajeError"] =
                    "Este movimiento ya fue finalizado.";

                return RedirectToAction("Index");
            }

            DateTime fechaSalida = DateTime.Now;

            TimeSpan permanencia =
                fechaSalida - movimiento.FechaHoraEntrada;

            double horasCobradas =
                Math.Ceiling(permanencia.TotalHours);

            if (horasCobradas < 1)
            {
                horasCobradas = 1;
            }

            decimal monto =
                (decimal)horasCobradas * 600;

            movimiento.FechaHoraSalida = fechaSalida;
            movimiento.EstadoMovimiento = "Finalizado";
            movimiento.MontoCobrado = monto;

            movimiento.EspacioEstacionamiento.Estado =
                "Disponible";

            db.SaveChanges();

            TempData["MensajeExito"] =
                "La salida se registró correctamente. " +
                "Monto a pagar: ₡" +
                monto.ToString("N2");

            return RedirectToAction("Details",
                new { id = movimiento.IdMovimiento });
        }

        // Función para cargar las listas de vehículos y espacios disponibles
        private void CargarListas(
        int? idVehiculoSeleccionado = null,
        int? idEspacioSeleccionado = null)
        {
            ViewBag.IdVehiculo = new SelectList(
                db.Vehiculos.OrderBy(v => v.Placa),
                "IdVehiculo",
                "Placa",
                idVehiculoSeleccionado
            );

            ViewBag.IdEspacio = new SelectList(
                db.Espacios
                    .Where(e =>
                        e.Estado == "Disponible" &&
                        e.Activo)
                    .OrderBy(e => e.CodigoEspacio),
                "IdEspacio",
                "CodigoEspacio",
                idEspacioSeleccionado
            );
        }

        // Función para mostrar el detalle de un movimiento.
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["MensajeError"] =
                    "Debe seleccionar un movimiento.";

                return RedirectToAction("Index");
            }

            MovimientoEstacionamiento movimiento =
                db.Movimientos
                    .Include(m => m.Vehiculo)
                    .Include(m => m.EspacioEstacionamiento)
                    .FirstOrDefault(m => m.IdMovimiento == id);

            if (movimiento == null)
            {
                TempData["MensajeError"] =
                    "El movimiento seleccionado no existe.";

                return RedirectToAction("Index");
            }

            return View(movimiento);
        }

        // Función para buscar movimientos mediante AJAX
        public ActionResult BuscarAjax(
            string criterio,
            string estado)
        {
            var movimientos = db.Movimientos
                .Include(m => m.Vehiculo)
                .Include(m => m.EspacioEstacionamiento)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(criterio))
            {
                string textoBuscado =
                    criterio.Trim().ToUpper();

                movimientos = movimientos.Where(m =>
                    m.Vehiculo.Placa
                        .ToUpper()
                        .Contains(textoBuscado) ||

                    m.EspacioEstacionamiento.CodigoEspacio
                        .ToUpper()
                        .Contains(textoBuscado)
                );
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                movimientos = movimientos.Where(m =>
                    m.EstadoMovimiento == estado
                );
            }

            return PartialView(
                "_TablaMovimientos",
                movimientos
                    .OrderByDescending(m => m.FechaHoraEntrada)
                    .ToList()
            );
        }

        // Función para mostrar la confirmación de eliminación de un movimiento.
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MovimientoEstacionamiento movimiento = db.Movimientos
                .Include(m => m.Vehiculo)
                .Include(m => m.EspacioEstacionamiento)
                .FirstOrDefault(m => m.IdMovimiento == id);

            if (movimiento == null)
            {
                return HttpNotFound();
            }

            return View(movimiento);
        }

        // Función para eliminar un movimiento finalizado.
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovimientoEstacionamiento movimiento = db.Movimientos.Find(id);

            if (movimiento == null)
            {
                return HttpNotFound();
            }

            if (movimiento.EstadoMovimiento == "Activo")
            {
                TempData["MensajeError"] =
                    "No es posible eliminar un movimiento que se encuentra activo.";

                return RedirectToAction("Index");
            }

            db.Movimientos.Remove(movimiento);
            db.SaveChanges();

            TempData["MensajeExito"] =
                "Movimiento eliminado correctamente.";

            return RedirectToAction("Index");
        }

        // Función para liberar los recursos utilizados por la conexión a la base de datos
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}