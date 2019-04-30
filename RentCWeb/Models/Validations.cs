using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RentCWeb.Models
{
    public class Validations
    {
        // NEW CAR RENT VALIDATIONS
        public Tuple<List<string>, bool> ValidateCarRent(string plate, string clientID, string startDate, string endDate, string city)
        {
            Reservation reservation = new Reservation();
            Car car = new Car();
            Customer customer = new Customer();
            DatabaseRequests request = new DatabaseRequests();
            List<string> errors = new List<string>();

              string plateError = "";
            string IDError = "";
            string startDateError = "";
            string endDateError = "";
            string cityError = "";
            string overlapDates = "";

            // Plate check
            plateError = CarPlateValidation(plate);

            // ClientID check
            IDError = ClientIDValidation(clientID);

            // Start-End Date check
            startDateError = DateValidation(startDate);

            if (startDateError == "")
            {
                startDateError = CompareDateValidation(DateTime.Today, DateTime.Parse(startDate));

                if (startDateError == "")
                {
                    endDateError = DateValidation(endDate);

                    if (endDateError == "")
                    {
                        endDateError = CompareDateValidation(DateTime.Parse(startDate), DateTime.Parse(endDate));

                        if (endDateError != "")
                        {
                            endDateError += " the start date";
                        }
                    }
                }
                else
                {
                    startDateError += " today";
                }
            }
            // Date Overlap check

            if (startDateError == "" && endDateError == "" && plateError == "")
                plateError = request.CheckDateOverlap(DateTime.Parse(startDate), DateTime.Parse(endDate), request.GetCarIDByPlate(plate));

            // City check
            cityError = LocationValidation(city);

            // Return true if all the data is correct/no error message
            if (plateError == "" && IDError == "" && startDateError == "" && endDateError == "" && cityError == "" && overlapDates == "")
            {

                return Tuple.Create(errors, true);
            }

            errors.Add(plateError);
            errors.Add(IDError);
            errors.Add(startDateError);
            errors.Add(endDateError);
            errors.Add(cityError);

            return Tuple.Create(errors, false); 
        }

        public string DateValidation(string checkedDate)
        {
            DateTime date;
            string str;
            if (string.IsNullOrWhiteSpace(checkedDate))
            {
                str = "This field is required";
            }
            else
            {
                str = (!DateTime.TryParseExact(checkedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ? "Please use the YYYY-MM-dd format" : "");
            }
            return str;
        }

        // CAR VALIDATIONS
        public string CarPlateValidation(string carPlate)
        {
            DatabaseRequests request = new DatabaseRequests();
            string errorMessage;

            if (string.IsNullOrWhiteSpace(carPlate))
            {
                errorMessage = "This field is required";
            }
            else if (!Regex.IsMatch(carPlate, @"[A-Za-z]{2}\s[0-9]{2}\s[A-Za-z]{3}"))
            {
                errorMessage = "Please use the correct format. E.g. AB 12 ABC";

            }
            else
            {
                errorMessage = request.GetCarIDByPlate(carPlate) == 0 ? "This Car Plate doesn't exist in the database" : "";
            }
            return errorMessage;
        }

        // CUSTOMER VALIDATIONS
        public string BirthDateValidation(DateTime birthDate)
        {
            string errorMessage = "";

            int age = DateTime.Now.Year - birthDate.Year;

            if (DateTime.Now.Month < birthDate.Month || (DateTime.Now.Month == birthDate.Month && DateTime.Now.Day < birthDate.Day))
                age--;

            if (age < 18)
                errorMessage = "Customer must be at least 18 years old";

            return errorMessage;
        }

        public string ClientIDValidation(string clientID)
        {
            DatabaseRequests request = new DatabaseRequests();
            string errorMessage;
            int ID;

            if (string.IsNullOrWhiteSpace(clientID))
            {
                errorMessage = "This field is required";
            }
            else if (!int.TryParse(clientID, out ID))
            {
                errorMessage = "Please use only numbers for the Client ID";
            }
            else
            {
                errorMessage = (request.GetClientID(clientID) == 0 ? "This Client ID doesn't exist" : "");
            }
            return errorMessage;
        }

        public string LocationValidation(string location)
        {
            string errorMessage;

            if (string.IsNullOrWhiteSpace(location))
            {
                errorMessage = "This field is required";
            }
            else if (location.Length > 50)
            {
                errorMessage = "Please introduce maximum 50 characters";
            }
            else
            {
                if (!Regex.IsMatch(location, @"^[\p{L}\s'.-]+$"))
                    errorMessage = "Please only use letters for the location";
                else
                    errorMessage = "";
            }

            return errorMessage;
        }

        public string NameValidation(string name)
        {
            string errorMessage;

            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "This field is required";
            }
            else if (!Regex.IsMatch(name, @"^[\p{L}\s'.-]+$"))
            {
                errorMessage = "Please only use letters for the name";
            }
            else if (name.Length > 50)
            {
                errorMessage = "Please introduce maximum 50 characters";
            }
            else
            {
                errorMessage = (!name.Trim().Contains(" ") ? "Please fill with the First and Last name" : "");
            }
            return errorMessage;
        }

        // RESERVATIONS VALIDATIONS
        public string CompareDateValidation(DateTime fixedDate, DateTime checkedDate)
        {
            if (DateTime.Compare(checkedDate, fixedDate) >= 0)
                return "";

            return "Please make sure the date is equal or bigger than ";
        }

        // NEW CUSTOMER VALIDATIONS
        public bool ValidateNewCustomer(string name, DateTime birthDate, string location)
        {
            Customer customer = new Customer();

            string nameError = "";
            string birthError = "";
            string locationError = "";

            // Name check
            nameError = NameValidation(name);

            // BirthDate check
            birthError = BirthDateValidation(birthDate);

            // Location check
            locationError = LocationValidation(location);


            if (nameError == "" && birthError == "" && locationError == "") return true;

            //NameErrorLabel.Text = nameError;
            //BirthErrorLabel.Text = birthError;
            //LocationErrorLabel.Text = locationError;

            return false;
        }

        //UPDATE CAR VALIDATIONS
        public bool ValidateUpdatedCarRent(string plate, string clientID, DateTime startDate, DateTime endDate, string city, string value)
        {
            if (value == "Clear")
            {
                Reservation reservation = new Reservation();
                Car car = new Car();
                Customer customer = new Customer();

                string plateError = "";
                string IDError = "";
                string startDateError = "";
                string endDateError = "";
                string cityError = "";

                // Plate check
                plateError = CarPlateValidation(plate);

                // ClientID check
                IDError = ClientIDValidation(clientID);

                // Start-End Date check
                startDateError = CompareDateValidation(DateTime.Now, startDate);

                if (startDateError == "")
                {
                    endDateError = CompareDateValidation(startDate, endDate);

                    if (endDateError != "")
                    {
                        endDateError += " the start date";
                    }
                }
                else
                {
                    startDateError += " the initial start date";
                }

                // City check
                cityError = LocationValidation(city);

                // Return true if all the data is correct/no error message
                if (plateError == "" && IDError == "" && startDateError == "" && endDateError == "" && cityError == "")
                {
                    return true;
                }

                //PlateErrorLabel.Text = plateError;
                //IDErrorLabel.Text = IDError;
                //StartErrorLabel.Text = startDateError;
                //EndErrorLabel.Text = endDateError;
                //CityErrorLabel.Text = cityError;

                return false;
            }
            else
            {
                //MessageBox.Show("Please Search for an active rent before pressing Update");
            }

            return false;
        }

        // UPDATE CUSTOMER VALIDATIONS
        public bool ValidateCustomer(string clientID, string name, DateTime birthDate, string location, Reservation initial)
        {
            Customer customer = new Customer();

            string nameError = "";
            string birthError = "";
            string locationError = "";

            // ID check
            if (clientID == initial.CustomerID.ToString())
            {
                // Name check
                nameError = NameValidation(name);

                // BirthDate check
                birthError = BirthDateValidation(birthDate);

                // Location check
                locationError = LocationValidation(location);


                if (nameError == "" && birthError == "" && locationError == "") return true;

                //NameErrorLabel.Text = nameError;
                //BirthErrorLabel.Text = birthError;
                //LocationErrorLabel.Text = locationError;

                return false;
            }
            else if (clientID == "")
            {
                //IDErrorLabel.Text = "This field is required";
                return false;
            }
            else
            {
                //MessageBox.Show("Please press Search before trying to update");
                return false;
            }

        }
    }
}