using ParqueoCentralWeb.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ParqueoCentralWeb.Controllers
{
    public class EspaciosController : Controller
    {
        private ParqueoContext db = new ParqueoContext();

        // Función para mostrar la lista de espacios de estacionamiento
        public ActionResult Index()
        {
            var espacios = db.Espacios
                .OrderBy(e => e.CodigoEspacio)
                .ToList();

            return View(espacios);
        }

        // Función para mostrar el formulario de registro de espacios
        public ActionResult Create()
        {
            return View();
        }

        // Función para guardar un nuevo espacio de estacionamiento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EspacioEstacionamiento espacio)
        {
            if (espacio.CodigoEspacio != null)
            {
                espacio.CodigoEspacio =
                    espacio.CodigoEspacio.Trim().ToUpper();
            }

            espacio.Estado = "Disponible";
            espacio.Activo = true;
            ModelState.Remove("Estado");

            bool codigoExiste = db.Espacios.Any(e =>
                e.CodigoEspacio == espacio.CodigoEspacio);

            if (codigoExiste)
            {
                ModelState.AddModelError(
                    "CodigoEspacio",
                    "Ya existe un espacio con este código."
                );
            }

            if (ModelState.IsValid)
            {
                db.Espacios.Add(espacio);
                db.SaveChanges();

                TempData["MensajeExito"] =
                    "El espacio se registró correctamente.";

                return RedirectToAction("Index");
            }

            return View(espacio);
        }

        // Función para mostrar el formulario de edición de un espacio
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["MensajeError"] =
                    "Debe seleccionar un espacio.";

                return RedirectToAction("Index");
            }

            EspacioEstacionamiento espacio =
                db.Espacios.Find(id);

            if (espacio == null)
            {
                TempData["MensajeError"] =
                    "El espacio seleccionado no existe.";

                return RedirectToAction("Index");
            }

            return View(espacio);
        }

        // Función para guardar los cambios realizados a un espacio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EspacioEstacionamiento espacio)
        {
            if (ModelState.IsValid)
            {
                EspacioEstacionamiento espacioExistente =
                    db.Espacios.Find(espacio.IdEspacio);

                if (espacioExistente == null)
                {
                    return HttpNotFound();
                }

                espacioExistente.TipoEspacio =
                    espacio.TipoEspacio;

                espacioExistente.Activo =
                    espacio.Activo;

                db.SaveChanges();

                TempData["MensajeExito"] =
                    "El espacio se actualizó correctamente.";

                return RedirectToAction("Index");
            }

            return View(espacio);
        }

        // Función para mostrar el detalle de un espacio
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["MensajeError"] =
                    "Debe seleccionar un espacio.";

                return RedirectToAction("Index");
            }

            EspacioEstacionamiento espacio =
                db.Espacios.Find(id);

            if (espacio == null)
            {
                TempData["MensajeError"] =
                    "El espacio seleccionado no existe.";

                return RedirectToAction("Index");
            }

            return View(espacio);
        }

        // Función para liberar los recursos utilizados por la conexión a la base de datos
        public ActionResult BuscarAjax(string criterio)
        {
            var espacios = db.Espacios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criterio))
            {
                string textoBuscado = criterio.Trim().ToUpper();

                espacios = espacios.Where(e =>
                    e.CodigoEspacio.ToUpper().Contains(textoBuscado) ||
                    e.TipoEspacio.ToUpper().Contains(textoBuscado) ||
                    e.Estado.ToUpper().Contains(textoBuscado) ||
                    (textoBuscado == "ACTIVO" && e.Activo) ||
                    (textoBuscado == "INACTIVO" && !e.Activo)
                );
            }

            return PartialView(
                "_TablaEspacios",
                espacios
                    .OrderBy(e => e.CodigoEspacio)
                    .ToList()
            );
        }

        // Función para mostrar la confirmación de eliminación de un espacio.
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EspacioEstacionamiento espacio = db.Espacios.Find(id);

            if (espacio == null)
            {
                return HttpNotFound();
            }

            return View(espacio);
        }

        // Función para eliminar un espacio si no tiene movimientos registrados.
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EspacioEstacionamiento espacio = db.Espacios.Find(id);

            if (espacio == null)
            {
                return HttpNotFound();
            }

            bool tieneMovimientos = db.Movimientos
                .Any(m => m.IdEspacio == id);

            if (tieneMovimientos)
            {
                TempData["MensajeError"] =
                    "No es posible eliminar el espacio porque tiene movimientos registrados.";

                return RedirectToAction("Index");
            }

            db.Espacios.Remove(espacio);
            db.SaveChanges();

            TempData["MensajeExito"] =
                "Espacio eliminado correctamente.";

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