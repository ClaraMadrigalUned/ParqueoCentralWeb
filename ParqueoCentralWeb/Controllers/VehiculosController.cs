using ParqueoCentralWeb.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ParqueoCentralWeb.Controllers
{
    public class VehiculosController : Controller
    {
        private ParqueoContext db = new ParqueoContext();

        // Función para mostrar la lista de vehículos y buscar por placa
        public ActionResult Index(string placa)
        {
            var vehiculos = db.Vehiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(placa))
            {
                string placaBuscada = placa.Trim().ToUpper();

                vehiculos = vehiculos.Where(v =>
                    v.Placa.Contains(placaBuscada));
            }

            return View(vehiculos.OrderBy(v => v.Placa).ToList());
        }

        // Función para mostrar el formulario de registro de vehículos
        public ActionResult Create()
        {
            return View();
        }

        // Función para guardar un nuevo vehículo en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vehiculo vehiculo)
        {
            if (vehiculo.Placa != null)
            {
                vehiculo.Placa = vehiculo.Placa.Trim().ToUpper();
            }

            bool placaExiste = db.Vehiculos.Any(v =>
                v.Placa == vehiculo.Placa);

            if (placaExiste)
            {
                ModelState.AddModelError(
                    "Placa",
                    "Ya existe un vehículo registrado con esta placa."
                );
            }

            if (ModelState.IsValid)
            {
                db.Vehiculos.Add(vehiculo);
                db.SaveChanges();

                TempData["MensajeExito"] =
                    "El vehículo se registró correctamente.";

                return RedirectToAction("Index");
            }

            return View(vehiculo);
        }

        // Función para mostrar el formulario de edición de un vehículo
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            Vehiculo vehiculo = db.Vehiculos.Find(id);

            if (vehiculo == null)
            {
                return HttpNotFound();
            }

            return View(vehiculo);
        }

        // Función para guardar los cambios realizados a un vehículo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                Vehiculo vehiculoExistente =
                    db.Vehiculos.Find(vehiculo.IdVehiculo);

                if (vehiculoExistente == null)
                {
                    return HttpNotFound();
                }

                vehiculoExistente.TipoVehiculo =
                    vehiculo.TipoVehiculo;

                vehiculoExistente.Propietario =
                    vehiculo.Propietario;

                vehiculoExistente.Contacto =
                    vehiculo.Contacto;

                db.SaveChanges();

                TempData["MensajeExito"] =
                    "La información del vehículo se actualizó correctamente.";

                return RedirectToAction("Index");
            }

            return View(vehiculo);
        }

        // Función para mostrar el detalle de un vehículo
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            Vehiculo vehiculo = db.Vehiculos.Find(id);

            if (vehiculo == null)
            {
                return HttpNotFound();
            }

            return View(vehiculo);
        }

        // Función para buscar vehículos mediante AJAX
        public ActionResult BuscarAjax(string placa)
        {
            var vehiculos = db.Vehiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(placa))
            {
                string placaBuscada = placa.Trim().ToUpper();

                vehiculos = vehiculos.Where(v =>
                    v.Placa.Contains(placaBuscada));
            }

            return PartialView(
                "_TablaVehiculos",
                vehiculos.OrderBy(v => v.Placa).ToList()
            );
        }

        // Función para mostrar la confirmación de eliminación de un vehículo.
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            Vehiculo vehiculo = db.Vehiculos.Find(id);

            if (vehiculo == null)
            {
                return HttpNotFound();
            }

            return View(vehiculo);
        }

        // Función para eliminar un vehículo si no tiene movimientos registrados.
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vehiculo vehiculo = db.Vehiculos.Find(id);

            if (vehiculo == null)
            {
                return HttpNotFound();
            }

            bool tieneMovimientos = db.Movimientos
                .Any(m => m.IdVehiculo == id);

            if (tieneMovimientos)
            {
                TempData["MensajeError"] =
                    "No es posible eliminar el vehículo porque tiene movimientos registrados.";

                return RedirectToAction("Index");
            }

            db.Vehiculos.Remove(vehiculo);
            db.SaveChanges();

            TempData["MensajeExito"] =
                "Vehículo eliminado correctamente.";

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