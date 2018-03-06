using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GarageProgram
{
    /// <summary>
    /// Runs the garage program and handles user interaction and input
    /// </summary>
    class GarageProgram
    {
        //we use this to store cars that have been created, but not added to the garage
        private static Dictionary<int, Car> orphanedCars = new Dictionary<int, Car>();

        //the garage object
        private static Garage MyGarage = new Garage();

        static void Main(string[] args)
        {
            Console.WriteLine("=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=");
            Console.WriteLine("=-=-=-=-=-=-=-WELCOME TO THE GARAGE-=-=-=-=-=-=-=-=");
            Console.WriteLine("=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=\n");

            bool showMenu = true;
            string[] menuOptions = { "1", "2", "3", "4", "5", "6" };

            while (showMenu)
            {
                Console.WriteLine("\n#######################################");
                Console.WriteLine("# What would you like to do?\n#");
                Console.WriteLine("# 1 - Create a car");
                Console.WriteLine("# 2 - Add a car to the garage");
                Console.WriteLine("# 3 - Remove a car from the garage");
                Console.WriteLine("# 4 - Search for a car in the garage");
                Console.WriteLine("# 5 - Display all cars in the garage");
                Console.WriteLine("# 6 - Exit the program\n#");
                Console.WriteLine("#######################################\n\n");

                string userSelection = Console.ReadLine();
                string[] carSettings;

                //if we didnt get a valid menu selection, try again
                if (menuOptions.Contains(userSelection) == false)
                {
                    Console.WriteLine("\n\n\nPLEASE SELECT A VALID MENU OPTION\n\n");

                }
                else
                {
                    switch (userSelection)
                    {
                        //creating a car
                        case "1":
                            carSettings = CreateCarMenu();

                            //if we didn't cancel out of create car menu, go ahead and create it
                            if (carSettings.Length != 0)
                            {
                                Car newCar = MyGarage.CreateCar(carSettings[0], carSettings[1], carSettings[2], orphanedCars);
                                //add it to the orphaned cars dictionary for safe keeping
                                orphanedCars.Add(newCar.getId(), newCar);
                            }
                            break;
                        
                        //add an existing car to the garage, or create a new car and add it to the garage
                        case "2":
                            Console.WriteLine("Press 'Enter' to add a new car, or enter the id of an existing car");
                            string userOption = Console.ReadLine();

                            //if user presses Enter, we get the CreateCarMenu
                            if (userOption.Equals(""))
                            {
                                carSettings = CreateCarMenu();

                                //user created a new car, and then we add it directly to the garage
                                if (carSettings.Length != 0)
                                {
                                    MyGarage.AddCar(MyGarage.CreateCar(carSettings[0], carSettings[1], carSettings[2], orphanedCars));
                                }
                            }
                            else
                            {
                                //user entered the id of an existing car (in orphaned cars dictionary)
                                int carKey = Int32.Parse(userOption);
                                if (orphanedCars.ContainsKey(carKey))
                                {
                                    MyGarage.AddCar((Car)orphanedCars[carKey]);
                                }
                                else
                                {
                                    Console.WriteLine("Sorry, unable to find a car with that id");
                                }
                            }
                            break;
                        
                        //remove a car from the garage
                        case "3":
                            Console.WriteLine("Enter the id of the car you wish to remove from the garage");
                            string userRemoveInput = Console.ReadLine();
                            MyGarage.RemoveCar(Int32.Parse(userRemoveInput));
                            break;
                        
                        //find a car by id
                        case "4":
                            Console.WriteLine("Enter the id of the car you wish to find in the garage");
                            string userSearchInput = Console.ReadLine();
                            MyGarage.SearchCar(Int32.Parse(userSearchInput));
                            break;
                        
                        //print all the cars in the garage out
                        case "5":
                            MyGarage.ShowcaseGarage();
                            break;
                        
                        //exit the program
                        case "6":
                            showMenu = false;
                            break;

                        //fallback that shouldn't be required
                        default:
                            Console.WriteLine("Sorry, unrecognized menu option");
                            break;

                    }//end switch

                }//end if

            }//end while

        }//end main

    
        /// <summary>
        /// Displays the create a car menu. This is needed in two places from Main, so break it away for reuse (DRY)
        /// </summary>
        /// <returns>string[] carSettings</returns>
        public static string[] CreateCarMenu()
        {
            string userConfirm = "N";
            string make = "";
            string model = "";
            string colour = "";

            //until we get a clear confirm or cancel, keep showing the menu
            while (!userConfirm.Equals("Y") && !userConfirm.Equals("y") && !userConfirm.Equals("X"))
            {
                Console.WriteLine("What is the make of the car?");
                make = Console.ReadLine();

                Console.WriteLine("What is the model of the car?");
                model = Console.ReadLine();

                Console.WriteLine("What is the colour of the car?");
                colour = Console.ReadLine();

                Console.WriteLine("Please confirm you want to create a {0} {1} {2} by typing 'Y'", colour, make, model);
                Console.WriteLine("You can cancel car creation by typing 'X', or any other key to start again");

                userConfirm = Console.ReadLine();
            }

            //if we got a 'Yes' entry, go ahead and return the settings the user inputed
            if (userConfirm.Equals("Y") || userConfirm.Equals("y"))
            {
                return new string[] { make, model, colour };
            }

            //return an empty array (resulting from an X entry)
            return new string[] { };
        }
    }


    /// <summary>
    /// Our Garage class allows the creation, storage, retrieval and removal of cars
    /// </summary>
    class Garage
    {
        //use a dictionary to store all of our garaged cars
        private Dictionary<int, Car> carList = new Dictionary<int, Car>();

        //the unique car ids
        private int carId = 0;


        /// <summary>
        /// Allows us to create a new car
        /// </summary>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="colour"></param>
        /// <param name="existingCars"></param>
        /// <returns>Car() newCar</returns>
        public Car CreateCar(string make, string model, string colour, Dictionary<int, Car> existingCars)
        {
            //generate an id
            bool nextId = true;

            //we let the hashtable handle the uniqueness of the ids
            while (nextId)
            {
                //we check the existing cars dictionary to see if the id exists
                if(existingCars.ContainsKey(this.carId))
                {
                    //we found a match, increment and try again
                    this.carId++;
                }
                else
                {
                    //no match, we can use the latest id
                    nextId = false;
                }
            }

            //create a new car and return it
            Car newCar = new Car(this.carId, make, model, colour);
            Console.WriteLine("\nCreated a new car: {0}", newCar.ToString());
            return newCar;
        }


        /// <summary>
        /// Allows adding a car object to the garage dictionary
        /// </summary>
        /// <param name="addCar"></param>
        public void AddCar(Car addCar)
        {
            //probably pedantic, but want to catch any issues
            try
            {
                //checked to make sure the carId doesn't already exist in the garage dictionary
                if (!carList.ContainsKey(addCar.getId()))
                {
                    carList[addCar.getId()] = addCar;
                    Console.WriteLine("\nSuccessfully added the car to the garage: {0} \n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n", addCar.ToString());
                }
                else
                {
                    Console.WriteLine("\nCar is already in the garage");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnable to add that car to the garage {0}", ex.Message);
            }

        }//end AddCar()


        /// <summary>
        /// Remove a car from the garage dictionary
        /// </summary>
        /// <param name="id"></param>
        public void RemoveCar(int id)
        {
            //make sure we can find this car in the garage
            if (carList.ContainsKey(id))
            {
                Car removeCar = (Car)carList[id];
                carList.Remove(id);
                Console.WriteLine("\nRemoved the following car from the garage: {0}", removeCar.ToString());
            }
            else
            {
                Console.WriteLine("\nSorry, no car matching that id could be found. \nUnable to remove a car that wasn't found\n");
            }
        }//end RemoveCar()


        /// <summary>
        /// Find a car in the garage by its id, and prints its attributes
        /// </summary>
        /// <param name="id"></param>
        public void SearchCar(int id)
        {
            if (carList.ContainsKey(id))
            {
                Console.WriteLine("\nFound a car: {0}", carList[id].ToString());
            }
            else
            {
                Console.WriteLine("\nSorry, no car matching that id could be found");
            }
        }//end SearchCar()


        /// <summary>
        /// Show all of the cars currently in the garage
        /// </summary>
        public void ShowcaseGarage()
        {
            //make sure we have at least 1 car
            if(carList.Count > 0)
            {
                Console.WriteLine("Showing all {0} cars in the garage\n", carList.Count);
                Console.WriteLine("\n-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n");
                foreach (KeyValuePair<int, Car> item in carList)
                {
                    Console.WriteLine(item.Value.ToString());
                }
                Console.WriteLine("\n-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n");
            }
            else
            {
                Console.WriteLine("There are no cars currently in the garage\n");
            }
           
        }//end ShowcaseGarage()

    }//end Garage()


    /// <summary>
    /// The Car class defines what a Car object will have and do
    /// </summary>
    class Car
    {
        private int id;
        private string make;
        private string model;
        private string colour;


        //constructor, we don't want a no-arg contructor
        public Car(int id, string make, string model, string colour)
        {
            //we dont want a setter as ID should never be changed
            this.id = id;

            //use the setters for these
            this.setMake(make);
            this.setModel(model);
            this.setColour(colour);
        }//end constructor


        /// <summary>
        /// Returns this car object
        /// </summary>
        /// <returns>Car() this</returns>
        public Car getCar()
        {
            return this;
        }


        /// <summary>
        /// Returns this cars id
        /// </summary>
        /// <returns>int id</returns>
        public int getId()
        {
            return this.id;
        }


        /// <summary>
        /// Returns this cars make attribute
        /// </summary>
        /// <returns>string make</returns>
        public string getMake()
        {
            return this.make;
        }


        /// <summary>
        /// Returns this cars model attribute
        /// </summary>
        /// <returns>string model</returns>
        public string getModel()
        {
            return this.model;
        }


        /// <summary>
        /// Returns this cars colour attribute
        /// </summary>
        /// <returns>string colour</returns>
        public string getColour()
        {
            return colour;
        }


        /// <summary>
        /// Sets this cars make attribute
        /// </summary>
        /// <param name="make"></param>
        public void setMake(string make)
        {
            this.make = make;
        }


        /// <summary>
        /// Sets this cars model attribute
        /// </summary>
        /// <param name="model"></param>
        public void setModel(string model)
        {
            this.model = model;
        }


        /// <summary>
        /// Sets this cars colour attribute
        /// </summary>
        /// <param name="colour"></param>
        public void setColour(string colour)
        {
            this.colour = colour;
        }


        /// <summary>
        /// Override of object.ToString to return a formatted attribute string
        /// describing this car
        /// </summary>
        /// <returns>string anon</returns>
        override public string  ToString()
        {
            return (string) ("[" + this.getId() + "] - " + this.getColour() + " " + this.getMake() + " " + this.getModel());
        }
    }//end Car()

}//end GarageProgram namespace

