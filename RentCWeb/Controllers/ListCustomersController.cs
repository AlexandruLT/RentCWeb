using RentCWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentCWeb.Controllers
{
    public class ListCustomersController : Controller
    {
        // GET: ListCustomers
        public ActionResult ListCustomers(string Button)
        {
            DatabaseRequests request = new DatabaseRequests();

            DataTable dt = request.GetCustomersList("all");

            if (Button.Equals("All"))
                dt = request.GetCustomersList("all");
            else if (Button.Equals("Active"))
                dt = request.GetCustomersList("active");
            else if (Button.Equals("Inactive"))
                dt = request.GetCustomersList("inactive");
            else
                return View("Menu");

            return View(dt);
        }
    }
}