using RentCWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentCWeb.Controllers
{
    public class ListAvailableCarsController : Controller
    {
        // GET: ListCars
        public ActionResult ListAvailableCars()
        {
            DatabaseRequests request = new DatabaseRequests();

            DataTable dt = request.GetCarsList();

            return View(dt);
        }
    }
}