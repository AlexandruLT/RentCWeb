using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RentCWeb.Models
{
    public class DatabaseRequests
    {
        //CAR REQUESTS
        public int GetCarIDByPlate(string plate)
        {
            DatabaseConnection connection = new DatabaseConnection();

            string getCarID = "SELECT CarID FROM Cars WHERE Plate=@Plate;";

            SqlCommand command = new SqlCommand(getCarID);
            command.Parameters.AddWithValue("Plate", plate);

            string value = connection.GetValueFromDatabase(command);

            if (value != "")
                return int.Parse(value);
            else
                return 0;
        }

        public string GetCarPlateByID(int carID)
        {
            DatabaseConnection connection = new DatabaseConnection();

            string getCarID = "SELECT Plate FROM Cars WHERE CarID = @CarID;";

            SqlCommand command = new SqlCommand(getCarID);
            command.Parameters.AddWithValue("CarID", carID);

            string value = connection.GetValueFromDatabase(command);

            if (value != "")
                return value;
            else
                return "";
        }

        //CUSTOMER REQUESTS
        public int GetClientID(string clientID)
        {
            DatabaseConnection connection = new DatabaseConnection();

            string getClientID = "SELECT CustomerID FROM Customers WHERE CustomerID=@ClientID AND IsDeleted = 0;";

            SqlCommand command = new SqlCommand(getClientID);

            command.Parameters.AddWithValue("ClientID", clientID);

            string value = connection.GetValueFromDatabase(command);

            if (value != "")
                return int.Parse(value);
            else
                return 0;
        }

        public string GetClientID()
        {

            DatabaseConnection connection = new DatabaseConnection();

            string getClientID = "SELECT IDENT_CURRENT('Customers');";

            SqlCommand command = new SqlCommand(getClientID);

            return (int.Parse(connection.GetValueFromDatabase(command)) + 1).ToString();
        }

        //RESERVATIONS REQUESTS
        public string CheckDateOverlap(DateTime startDate, DateTime endDate, int carID)
        {
            DatabaseConnection connection = new DatabaseConnection();

            List<DateTime> dates = connection.GetDatesFromDatabase(carID);

            if (dates.Count > 0)
                for (int i = 0; i < dates.Count; i += 2)
                {
                    if (!(startDate > dates[i + 1] || endDate < dates[i]))
                        return "This car is rented between " + dates[i].Date.ToShortDateString() + " and " + dates[i + 1].Date.ToShortDateString();
                }

            return "";
        }

        //LIST CARS REQUESTS
        public DataTable GetCarsList()
        {
            DatabaseConnection connection = new DatabaseConnection();

            string getCarsList = "SELECT CarID as 'Car ID', Plate, Manufacturer, Model, PricePerDay as 'Price Per Day' FROM Cars order by CarID";

            return connection.GetTableFromDatabase(getCarsList);
        }

        //LIST CUSTOMERS REQUESTS
        public DataTable GetCustomersList(string lookFor)
        {
            DatabaseConnection connection = new DatabaseConnection();

            string getCustomersList = "";

            if (lookFor == "all")
                getCustomersList = "SELECT CustomerID as 'Client ID', NAme as 'Client Name', BirthDate as 'Birth Date', Location FROM Customers order by CustomerID";
            else if (lookFor == "active")
                getCustomersList = "SELECT CustomerID as 'Client ID', NAme as 'Client Name', BirthDate as 'Birth Date', Location FROM Customers WHERE IsDeleted = 0 order by CustomerID";
            else if (lookFor == "inactive")
                getCustomersList = "SELECT CustomerID as 'Client ID', NAme as 'Client Name', BirthDate as 'Birth Date', Location FROM Customers WHERE IsDeleted = 1 order by CustomerID";

            return connection.GetTableFromDatabase(getCustomersList);
        }

        //LIST RENTS REQUESTS
        public DataTable GetRentsList(string lookFor)
        {
            DatabaseConnection connection = new DatabaseConnection();
            string getRentsList = "";

            if (lookFor == "active")
                getRentsList = "SELECT Plate as 'Car Plate', CustomerID as 'Customer ID', StartDate as 'Start Date', EndDate as 'End Date', Location " +
                                "FROM(SELECT CarID, CustomerID, StartDate, EndDate, Location FROM Reservations WHERE ReservStatsID = 1) " +
                                "Reservations LEFT JOIN Cars on Reservations.CarID = Cars.CarID order by Reservations.CarID";

            else if (lookFor == "inactive")
                getRentsList = "SELECT Plate as 'Car Plate', CustomerID as 'Customer ID', StartDate as 'Start Date', EndDate as 'End Date', Location " +
                                "FROM(SELECT CarID, CustomerID, StartDate, EndDate, Location FROM Reservations WHERE ReservStatsID = 2) " +
                                "Reservations LEFT JOIN Cars on Reservations.CarID = Cars.CarID order by Reservations.CarID";

            else if (lookFor == "deleted")
                getRentsList = "SELECT Plate as 'Car Plate', CustomerID as 'Customer ID', StartDate as 'Start Date', EndDate as 'End Date', Location " +
                                "FROM(SELECT CarID, CustomerID, StartDate, EndDate, Location FROM Reservations WHERE ReservStatsID = 3) " +
                                "Reservations LEFT JOIN Cars on Reservations.CarID = Cars.CarID order by Reservations.CarID";

            else
                getRentsList = "SELECT Plate as 'Car Plate', CustomerID as 'Customer ID', StartDate as 'Start Date', EndDate as 'End Date', Location " +
                                "FROM(SELECT CarID, CustomerID, StartDate, EndDate, Location FROM Reservations WHERE ReservStatsID = 1 OR ReservStatsID = 2) " +
                                "Reservations LEFT JOIN Cars on Reservations.CarID = Cars.CarID order by Reservations.CarID";

            return connection.GetTableFromDatabase(getRentsList);
        }

        // NEW CAR RENT REQUESTS
        // Inserting the new rent in the Database
        public void AddCarRentToDatabase(string plate, string clientID, DateTime startDate, DateTime endDate, string city)
        {
            DatabaseConnection connection = new DatabaseConnection();
            Car car = new Car();
            Customer customer = new Customer();

            string addRent = "INSERT INTO Reservations " +
                             "VALUES(@CarID,@CustomerID, 1, @StartDate, @EndDate, @Location, NULL);";

            SqlCommand command = new SqlCommand(addRent);

            car.CarID = GetCarIDByPlate(plate);

            command.Parameters.AddWithValue("@CarID", car.CarID);
            command.Parameters.AddWithValue("@CustomerID", int.Parse(clientID));
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);
            command.Parameters.AddWithValue("@Location", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.ToLower()));

            connection.AddDataToDatabase(command);
        }

        // NEW CUSTOMER REQUESTS
        public void AddCustomerToDatabse(string name, DateTime birthDate, string location)
        {
            DatabaseConnection connection = new DatabaseConnection();

            string addRent = "INSERT INTO Customers " +
                             "VALUES(@Name, @BirthDate, @Location, 0);";

            SqlCommand command = new SqlCommand(addRent);

            command.Parameters.AddWithValue("@Name", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()));
            command.Parameters.AddWithValue("@BirthDate", birthDate);
            command.Parameters.AddWithValue("@Location", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(location.ToLower()));

            connection.AddDataToDatabase(command);
        }

        // UPDATE CAR RENT REQUESTS
        public void UpdateCarRentInDatabase(string plate, string clientID, DateTime startDate, DateTime endDate, string city, Reservation initialRes)
        {
            Car car = new Car();
            DatabaseConnection connection = new DatabaseConnection();

            string addRent = "UPDATE Reservations " +
                                "SET StartDate = @StartDate, EndDate = @EndDate, Location = @Location WHERE CarID = @CarID AND CustomerID = @CustomerID AND StartDate = @InitialStartDate";

            SqlCommand command = new SqlCommand(addRent);

            car.CarID = GetCarIDByPlate(plate);

            command.Parameters.AddWithValue("CarID", car.CarID);
            command.Parameters.AddWithValue("CustomerID", int.Parse(clientID));
            command.Parameters.AddWithValue("StartDate", startDate);
            command.Parameters.AddWithValue("InitialStartDate", initialRes.StartDate);
            command.Parameters.AddWithValue("EndDate", endDate);
            command.Parameters.AddWithValue("Location", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.ToLower()));

            connection.AddDataToDatabase(command);
        }

        public string GetRentForUpdate(string plate, string clientID, string startDate, CarRent rent)
        {

            if (plate != null || clientID != "0")
            {
                Car car = new Car();
                Customer customer = new Customer();
                Validations validation = new Validations();
                DatabaseRequests request = new DatabaseRequests();
                    

                string IDError = "";
                string plateError = "";

                if (plate != null)
                {
                    plateError = validation.CarPlateValidation(plate);
                }
                else
                {
                plateError = "This field is required";
                }
                if (clientID != "0")
                {
                    IDError = validation.ClientIDValidation(clientID);
                }
                else
                {
                    IDError = "This field is required";
                }

                if ((plateError == "" && IDError == "This field is required") || (IDError == "" && plateError == "This field is required") || (plateError == "" && IDError == ""))
                {
                    DatabaseConnection connection = new DatabaseConnection();
                    List<Reservation> res = new List<Reservation>();

                    string getRent = "";
                    string lookFor = "";
                    string dates = "";

                    if (plateError == "" && IDError == "" && startDate != null)
                    {
                        getRent = "SELECT CarID, CustomerID, StartDate, EndDate, Location FROM Reservations WHERE CustomerID = @CustomerID AND CarID = @CarID AND StartDate = @StartDate AND ReservStatsID = 1";
                        lookFor = "plateAndIDAndDate";
                    }
                    else if (plateError == "" && IDError == "")
                    {
                        getRent = "SELECT CarID, CustomerID, StartDate, EndDate, Location FROM Reservations WHERE CustomerID = @CustomerID AND CarID = @CarID AND ReservStatsID = 1";
                        lookFor = "plateAndID";
                    }
                    else if (plateError == "")
                    {
                        getRent = "SELECT CarID, CustomerID, StartDate, EndDate, Location FROM Reservations WHERE CarID = @CarID AND ReservStatsID = 1";
                        lookFor = "plate";
                    }
                    else if (IDError == "")
                    {
                        getRent = "SELECT CarID, CustomerID, StartDate, EndDate, Location FROM Reservations WHERE CustomerID = @CustomerID AND ReservStatsID = 1";
                        lookFor = "ID";
                    }

                    SqlCommand command = new SqlCommand(getRent);

                    if (lookFor == "plateAndID" && plateError == "" && IDError == "")
                    {
                        car.CarID = GetCarIDByPlate(plate);

                        command.Parameters.AddWithValue("CarID", car.CarID);
                        command.Parameters.AddWithValue("CustomerID", int.Parse(clientID));
                    }
                    else if (lookFor == "plate" && plateError == "")
                    {
                        car.CarID = GetCarIDByPlate(plate);

                        command.Parameters.AddWithValue("CarID", car.CarID);
                    }
                    else if (lookFor == "ID" && IDError == "")
                    {
                        command.Parameters.AddWithValue("CustomerID", int.Parse(clientID));
                    }
                    else if (lookFor == "plateAndIDAndDate" && plateError == "" && IDError == "")
                    {
                        car.CarID = GetCarIDByPlate(plate);

                        command.Parameters.AddWithValue("CarID", car.CarID);
                        command.Parameters.AddWithValue("CustomerID", int.Parse(clientID));
                        command.Parameters.AddWithValue("StartDate", DateTime.Parse(startDate));
                    }


                    res = connection.GetRentForUpdate(command);
                    IDError = "";
                    plateError = "";

                    string errorMessage = "";

                    if (lookFor == "plate")
                    {
                        if (res.Count > 1)
                        {
                            errorMessage = "This car is rented more than 1 time. Please add the Client ID field and press Search again";
                        }
                        else if (res.Count == 0)
                        {
                            errorMessage = "There is no active rent for this car";
                        }
                    }
                    else if (lookFor == "ID")
                    {
                        if (res.Count > 1)
                        {
                            errorMessage = "This client has multiple active rents. Please add the Car Plate and press Search again";
                        }
                        else if (res.Count == 0)
                        {
                            errorMessage = "There is no active rent for this customer";
                        }
                    }
                    else if (lookFor == "plateAndID")
                    {
                        if (res.Count > 1)
                        {
                            for (int i = 0; i < res.Count; i++)
                                dates += "\n" + res[i].StartDate.ToShortDateString();

                            errorMessage = "This car is rented for this customer on the following start dates :" + dates + "\nPlease select one of these dates as a start date before pressing Search.";
                        }
                        else if (res.Count == 0)
                        {
                            errorMessage = "There is no active rent for this customer/car";
                        }
                    }
                    else
                    {
                        //StartDateTimePicker.CustomFormat = " ";
                        //StartDateTimePicker.Checked = false;

                        errorMessage = "There isn't a rent for this client and car on this date. Please press Search again using only the Car Plate and Client ID";
                    }

                    if (res.Count == 1)
                    {
                    //StartDateTimePicker.Checked = true;
                    //StartDateTimePicker.CustomFormat = "yyyy-MM-dd";
                    //EndDateTimePicker.CustomFormat = "yyyy-MM-dd";

                        rent.Reservation.CustomerID = res[0].CustomerID;
                        rent.Car.Plate = request.GetCarPlateByID(res[0].CarID);
                        rent.StartDate = res[0].StartDate.ToShortDateString();
                        rent.EndDate = res[0].EndDate.ToShortDateString();
                        rent.Reservation.Location = res[0].Location;
                    }
                    else
                    {
                        return errorMessage;
                    }
                }

                return IDError + "/" + plateError;
                //IDErrorLabel.Text = IDError;
                //PlateErrorLabel.Text = plateError;
            }
                else
                {
                return  "Please fill the Car Plate field or the Client ID field before pressing Search";
                }
        }

        public string DeleteRent(string plate, string clientID, string value, Reservation initialRes)
        {
            if (value == "Clear")
            {
                Car car = new Car();
                DatabaseConnection connection = new DatabaseConnection();

                string addRent = "UPDATE Reservations " +
                                 "SET ReservStatsID = 3 WHERE CarID = @CarID AND CustomerID = @CustomerID AND StartDate = @InitialStartDate";

                SqlCommand command = new SqlCommand(addRent);

                car.CarID = GetCarIDByPlate(plate);

                command.Parameters.AddWithValue("CarID", car.CarID);
                command.Parameters.AddWithValue("CustomerID", int.Parse(clientID));
                command.Parameters.AddWithValue("InitialStartDate", initialRes.StartDate);

                connection.AddDataToDatabase(command);

                return "Rent Deleted";
            }
            else
                return "Please Search for an active Rent before pressing Delete";
        }

        //UPDATE CUSTOMER REQUESTS
        public int GetCustomerForUpdate(string clientID)
        {

            DatabaseConnection connection = new DatabaseConnection();
            Customer initialCustomer = new Customer();

            //IDErrorLabel.Text = initialCustomer.ClientIDValidation(clientID);
            //if (IDErrorLabel.Text == "")
            {
                string getCustomer = "SELECT Name, BirthDate, Location from Customers WHERE CustomerID = @CustomerID AND IsDeleted = 0";

                SqlCommand command = new SqlCommand(getCustomer);

                command.Parameters.AddWithValue("CustomerID", int.Parse(clientID));

                initialCustomer = connection.GetCustomerForUpdate(command);

                //initialCustomer.clientID = int.Parse(clientID);
                //NameTextBox.Text = initialCustomer.name;
                //BirthDateTimePicker.Value = initialCustomer.birthDate;
                //LocationTextBox.Text = initialCustomer.location;

                return initialCustomer.CustomerID;
            }


            return 0;
        }

        public void UpdateCustomerInDatabase(string clientID, string name, DateTime birthDate, string location)
        {
            DatabaseConnection connection = new DatabaseConnection();

            string updateCustomer = "UPDATE Customers " +
                             "SET Name=@Name, BirthDate=@BirthDate, Location=@Location " +
                             "WHERE CustomerID=@CustomerID;";

            SqlCommand command = new SqlCommand(updateCustomer);

            command.Parameters.AddWithValue("CustomerID", int.Parse(clientID));
            command.Parameters.AddWithValue("Name", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()));
            //command.Parameters.AddWithValue("BirthDate", BirthDateTimePicker.Value.Date);
            command.Parameters.AddWithValue("Location", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(location.ToLower()));

            connection.AddDataToDatabase(command);

            //MessageBox.Show("Customer Updated");
        }

        public void DeleteCustomerInDatabase(string clientID)
        {
            DatabaseConnection connection = new DatabaseConnection();

            string deleteCustomer = "UPDATE Customers " +
                             "SET IsDeleted = 1 " +
                             "WHERE CustomerID=@CustomerID;";

            SqlCommand command = new SqlCommand(deleteCustomer);

            command.Parameters.AddWithValue("CustomerID", int.Parse(clientID));

            connection.AddDataToDatabase(command);

            //MessageBox.Show("Customer Deleted");
        }

        public void DeleteCustomer(string clientID)
        {
            //if (clientID.Trim() == initialID.ToString())
            //{
            //    if (MessageBox.Show("Are you sure you want to delete this customer?", "Delete Customer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //        DeleteCustomerInDatabase(clientID);
            //}
            //else if (clientID.Trim() == "")
            //{
            //    IDErrorLabel.Text = "This field is required";
            //}
            //else
            //{
            //    MessageBox.Show("Please press Search before trying to update");
            //}
        }
    }
}