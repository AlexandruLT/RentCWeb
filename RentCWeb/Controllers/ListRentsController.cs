using RentCWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentCWeb.Controllers
{
    public class ListRentsController : Controller
    {
        // GET: ListRents
        public ActionResult ListRents(string Button)
        {
            DatabaseRequests request = new DatabaseRequests();
            DataTable dt = new DataTable();
            if (Button.Equals("All"))
                dt = request.GetRentsList("all");
            else if (Button.Equals("Active"))
                dt = request.GetRentsList("active");
            else if (Button.Equals("Inactive"))
                dt = request.GetRentsList("inactive");
            else if (Button.Equals("Deleted"))
                dt = request.GetRentsList("deleted");
            else
                return View("Menu");

            return View(dt);
        }
    }
}