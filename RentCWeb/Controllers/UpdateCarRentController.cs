using RentCWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentCWeb.Controllers
{
    public class UpdateCarRentController : Controller
    {
        // GET: UpdateCarRent
        public ActionResult UpdateCarRent(CarRent rent, string Button)
        {
            Validations validation = new Validations();
            DatabaseRequests request = new DatabaseRequests();
            Reservation initialRes = new Reservation();
            string error = "";

            if (Button.Equals("Search"))
            {
                if (validation.DateValidation(rent.StartDate) == "")
                    error = request.GetRentForUpdate(rent.Car.Plate, rent.Reservation.CustomerID.ToString(), rent.StartDate, rent);
                else
                    error = request.GetRentForUpdate(rent.Car.Plate, rent.Reservation.CustomerID.ToString(), rent.StartDate, rent);

                if (error == "/")
                {
                    initialRes = rent.Reservation;
                    initialRes.StartDate = DateTime.Parse(rent.StartDate);
                }
                else if (error.Contains("/"))
                {
                    string[] errors = error.Split('/');

                    if (errors[0] != "")
                        ModelState.AddModelError("CustomerID", errors[0]);
                    if (errors[1] != "")
                        ModelState.AddModelError("Plate", errors[1]);
                }
                else
                {
                    ModelState.AddModelError("Plate", error);
                }
            }
            else if (Button.Equals("Delete"))
            {
                if (initialRes.Location != "")
                    error = request.DeleteRent(rent.Car.Plate, rent.Reservation.CustomerID.ToString(), Button, initialRes);
            }
            else if (Button.Equals("Update"))
            {
                Tuple<List<string>, bool> value = validation.ValidateCarRent(rent.Car.Plate, rent.Reservation.CustomerID.ToString(), rent.StartDate, rent.EndDate, rent.Reservation.Location);
                if (value.Item2)
                {
                    request.UpdateCarRentInDatabase(rent.Car.Plate, rent.Reservation.CustomerID.ToString(), DateTime.Parse(rent.StartDate), DateTime.Parse(rent.EndDate), rent.Reservation.Location, initialRes);
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
                }
            }
            else
            {
                return View("Menu");
            }

            return View("UpdateCarRent", rent);
        }
    }
}