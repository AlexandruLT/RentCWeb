using RentCWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentCWeb.Controllers
{
    public class RegisterNewCarRentController : Controller
    {
        // GET: RegisterNewCarRent
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegisterNewCarRent(CarRent rent, string Button)
        {
            if (Button.Equals("Submit"))
            {
                Validations validation = new Validations();
                DatabaseRequests request = new DatabaseRequests();
                Tuple<List<string>, bool> value = validation.ValidateCarRent(rent.Car.Plate, rent.Reservation.CustomerID.ToString(), rent.StartDate, rent.EndDate, rent.Reservation.Location);

                if (value.Item2)
                {
                    request.AddCarRentToDatabase(rent.Car.Plate, rent.Reservation.CustomerID.ToString(), DateTime.Parse(rent.StartDate), DateTime.Parse(rent.EndDate), rent.Reservation.Location);
                }
                else
                {
                    if (value.Item1[0] != "")
                        ModelState.AddModelError("Plate", value.Item1[0]);
                    if (value.Item1[1] != "")
                        ModelState.AddModelError("CustomerID", value.Item1[1]);
                    if (value.Item1[2] != "")
                        ModelState.AddModelError("StartDate", value.Item1[2]);
                    if (value.Item1[3] != "")
                        ModelState.AddModelError("EndDate", value.Item1[3]);
                    if (value.Item1[4] != "")
                        ModelState.AddModelError("Location", value.Item1[4]);

                    return View("RegisterNewCarRent");
                }

            }

            return View("Menu");
        }


    }
}