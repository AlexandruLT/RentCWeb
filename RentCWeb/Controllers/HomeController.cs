using RentCWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentCWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult RegisterNewCarRent()
        {

            return View();
        }

        public ActionResult UpdateCarRent()
        {
            return View();
        }

        public ActionResult ListRents()
        {
            DatabaseRequests request = new DatabaseRequests();

            DataTable dt = request.GetRentsList("all");

            return View(dt);
        }

        public ActionResult ListAvailableCars()
        {
            DatabaseRequests request = new DatabaseRequests();

            DataTable dt = request.GetCarsList();

            return View(dt);
        }

        public ActionResult RegisterNewCustomer()
        {
            return View();
        }

        public ActionResult UpdateCustomer()
        {
            return View();
        }

        public ActionResult ListCustomers()
        {
            DatabaseRequests request = new DatabaseRequests();

            DataTable dt = request.GetCustomersList("all");

            return View(dt);
        }
    }
}