using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ParqueoCentralWeb.Controllers
{
    public class HomeController : Controller
    {
        // Función para mostrar la página principal.
        public ActionResult Index()
        {
            return View();
        }

        // Función para guardar el nombre del operador en la sesión.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarOperador(string nombreOperador)
        {
            if (!string.IsNullOrWhiteSpace(nombreOperador))
            {
                Session["NombreOperador"] = nombreOperador.Trim();
            }

            return RedirectToAction("Index");
        }
    }
}