using System;
using System.Collections.Generic;
using Roommates.Repositories;
using Roommates.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;

namespace Roommates
{
    public class Program
    {
        //  This is the address of the database.
        //  We define it here as a constant since it will never change.
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true; TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            //create a new instance of a RoomRepository 
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);


            ChoreRepository choreRepo = new ChoreRepository(CONNECTION_STRING);

            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);


            bool runProgram = true;
            while (runProgram)
            {
                string selection = GetMenuSelection();

                switch (selection)
                {
                    case ("Show all rooms"):
                        // Do stuff
                        List<Room> rooms = roomRepo.GetAll();
                        foreach (Room r in rooms)
                        {
                            Console.WriteLine($"{r.Name} has an Id of {r.Id} and a max occupancy of {r.MaxOccupancy}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for room"):
                        // Do stuff
                        Console.Write("Room Id: ");
                        int id = int.Parse(Console.ReadLine());

                        Room room = roomRepo.GetById(id);

                        Console.WriteLine($"{room.Id} - {room.Name} Max Occupancy({room.MaxOccupancy})");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Add a room"):
                        // Do stuff
                        Console.Write("Room name: ");
                        string name = Console.ReadLine();

                        Console.Write("Max occupancy: ");
                        int max = int.Parse(Console.ReadLine());

                        Room roomToAdd = new Room()
                        {
                            Name = name,
                            MaxOccupancy = max
                        };

                        roomRepo.Insert(roomToAdd);

                        Console.WriteLine($"{roomToAdd.Name} has been added and assigned an Id of {roomToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;




                    case ("Show all chores"):
                        // Do stuff
                        List<Chore> chores = choreRepo.GetAll();
                        foreach (Chore c in chores)
                        {
                            Console.WriteLine($"{c.Name} has an Id of {c.Id}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for chore"):
                        // Do stuff
                        Console.Write("Chore Id: ");
                        int choreId = int.Parse(Console.ReadLine()); //change name to choreId

                        Chore chore = choreRepo.GetById(choreId);

                        Console.WriteLine($"{chore.Id} - {chore.Name}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Add a chore"):
                        // Do stuff
                        Console.Write("Chore name: ");
                        string choreName = Console.ReadLine();


                        Chore choreToAdd = new Chore()
                        {
                            Name = choreName,
                        };

                        choreRepo.Insert(choreToAdd);

                        Console.WriteLine($"{choreToAdd.Name} has been added and assigned an Id of {choreToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Show unassigned chores"):
                        List<Chore> UnassignedChores = choreRepo.GetUnassignedChores();
                        foreach (Chore uc in UnassignedChores)
                        {
                            Console.WriteLine($"{uc.Name}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;



                    case ("Search for roommate"):
                        // Do stuff
                        Console.Write("Roommate Id: ");
                        int roommateId = int.Parse(Console.ReadLine()); //change name to roommateId

                        Roommate roommate = roommateRepo.GetById(roommateId);

                        Console.WriteLine($"{roommate.FirstName} - Rent Portion: {roommate.RentPortion} - Room Name: {roommate.Room.Name}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;





                    case ("Show all roommates"):
                        // Do stuff
                        List<Roommate> roommates = roommateRepo.GetAll();
                        foreach (Roommate r in roommates)
                        {
                            Console.WriteLine($"{r.FirstName} {r.LastName} has a rent portion of {r.RentPortion}, move in date of {r.MoveInDate} and an Id of {r.Id}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;



                    case ("Search for roommates using room Id"):
                        // Do stuff
                        Console.Write("Room Id: ");
                        int roomId = int.Parse(Console.ReadLine()); //change name to roommateId

                        // Fetch the list of roommates associated with the given room ID
                        List<Roommate> roommatesList = roommateRepo.GetRoommatesByRoomId(roomId);

                        // Iterate through the list and display each roommate's details
                        foreach (Roommate r in roommatesList)
                        {
                            Console.WriteLine($"{r.FirstName} {r.LastName} - Rent Portion: {r.RentPortion}, Move-In Date: {r.MoveInDate.ToShortDateString()}");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;








                    case ("Add a roommate"):
                        // Do stuff
                        Console.Write("First name: ");
                        string firstName = Console.ReadLine();
                        Console.Write("Last name: ");
                        string lastName = Console.ReadLine();
                        Console.Write("Rent portion: ");
                        int rentPortion = int.Parse(Console.ReadLine());
                        Console.Write("Move in date (yyyy-MM-dd): "); // Include date format for clarity
                        DateTime moveInDate = DateTime.Parse(Console.ReadLine());
                        Console.Write("Room Id: ");
                        int roomIdForNewRoommate = int.Parse(Console.ReadLine());

                        Roommate roommateToAdd = new Roommate()
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            RentPortion = rentPortion,
                            MoveInDate = moveInDate,
                            // Assign the roomId directly to the RoomId property
                            Room = new Room { Id = roomIdForNewRoommate } // Set Room property with RoomId only if it's used in the Insert method

                        };

                        roommateRepo.Insert(roommateToAdd);

                        Console.WriteLine($"{roommateToAdd.FirstName} {roommateToAdd.LastName} with rent portion of {roommateToAdd.RentPortion} and move in date of {roommateToAdd.MoveInDate} has been added and assigned an Id of {roommateToAdd.Id}"); //Note: .Id
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;


                    case ("Update roommate"):
                        // Fetch all roommates
                        List<Roommate> roommateOptions = roommateRepo.GetAll();
                        foreach (Roommate r in roommateOptions)
                        {
                            // check null Room property
                            string assignedRoomId = r.Room != null ?$"{r.Room?.Id}" : "No Room Assigned";
                            Console.WriteLine($"{r.Id} - {r.FirstName} - {r.LastName} - Rent Portion: {r.RentPortion} - Move In Date: {r.MoveInDate:MM/dd/yyyy} - Room ID: {assignedRoomId}");
                        }

                        Console.Write("Which roommate would you like to update? (Enter roommate id) ");
                        if (!int.TryParse(Console.ReadLine(), out int selectedRoommateId))
                        {
                            Console.WriteLine("Invalid roommate ID.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        Roommate selectedRoommate = roommateOptions.FirstOrDefault(r => r.Id == selectedRoommateId);

                        if (selectedRoommate == null)
                        {
                            Console.WriteLine("Roommate not found.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        // Display current values before update
                        Console.WriteLine($"Updating Roommate: Id={selectedRoommate.Id}, FirstName={selectedRoommate.FirstName}, LastName={selectedRoommate.LastName}, RentPortion={selectedRoommate.RentPortion}, MoveInDate={selectedRoommate.MoveInDate}, RoomId={selectedRoommate.Room.Id}");

                        // Update roommate details
                        Console.Write("New First Name: ");
                        selectedRoommate.FirstName = Console.ReadLine();

                        Console.Write("New Last Name: ");
                        selectedRoommate.LastName = Console.ReadLine();

                        Console.Write("New Rent Portion: ");
                        if (!int.TryParse(Console.ReadLine(), out int newRentPortion))
                        {
                            Console.WriteLine("Invalid rent portion.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }
                        selectedRoommate.RentPortion = newRentPortion;

                        Console.Write("New Move In Date (yyyy-MM-dd): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime newMoveInDate))
                        {
                            Console.WriteLine("Invalid date format.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }
                        selectedRoommate.MoveInDate = newMoveInDate;

                        Console.Write("New Room Id (press Enter to leave unchanged): ");
                        string roomIdInput = Console.ReadLine();
                        if (int.TryParse(roomIdInput, out int newRoomId))
                        {
                            selectedRoommate.Room = new Room { Id = newRoomId };
                        }

                        // Display updated values before calling Update method
                        Console.WriteLine($"Final Values for Update: Id={selectedRoommate.Id}, FirstName={selectedRoommate.FirstName}, LastName={selectedRoommate.LastName}, RentPortion={selectedRoommate.RentPortion}, MoveInDate={selectedRoommate.MoveInDate}, RoomId={selectedRoommate.Room?.Id}");

                        roommateRepo.Update(selectedRoommate);

                        Console.WriteLine("Roommate has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;




                    case ("Delete roommate"):
                        // Fetch all roommates
                        List<Roommate> roommateOptions1 = roommateRepo.GetAll();

                        //show list of roommates + info:
                        foreach (Roommate r in roommateOptions1)
                        {
                            // check null Room property
                            string assignedRoomId = r.Room != null ? $"{r.Room?.Id}" : "No Room Assigned";
                            Console.WriteLine($"{r.Id} - {r.FirstName} - {r.LastName} - Rent Portion: {r.RentPortion} - Move In Date: {r.MoveInDate:MM/dd/yyyy} - Room ID: {assignedRoomId}");
                        }

                        Console.Write("Which roommate would you like to delete? (Enter roommate id) ");
                        if (!int.TryParse(Console.ReadLine(), out int selectedRoommateId1))
                        {
                            Console.WriteLine("Invalid roommate ID.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        Roommate selectedRoommate1 = roommateOptions1.FirstOrDefault(r => r.Id == selectedRoommateId1);

                        if (selectedRoommate1 == null)
                        {
                            Console.WriteLine("Roommate not found.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }
                        // Display updated values before calling Update method
                        //Console.WriteLine($"Are you sure you want to delete: Id={selectedRoommate1.Id}, FirstName={selectedRoommate1.FirstName}, LastName={selectedRoommate1.LastName}, RentPortion={selectedRoommate1.RentPortion}, MoveInDate={selectedRoommate1.MoveInDate}, RoomId={selectedRoommate1.Room?.Id} (y/n) ?");

                        roommateRepo.Delete(selectedRoommate1.Id);



                        Console.WriteLine("Roommate has been successfully deleted");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;






                    //first show list of all rooms and ask to select which to update.
                    //It then collects what they'd like the updated Name and Max Occupancy to be, and finally saves those changes to the database.
                    case ("Update a room"):
                        List<Room> roomOptions = roomRepo.GetAll();
                        foreach (Room r in roomOptions)
                        {
                           
                            Console.WriteLine($"{r.Id} - {r.Name} Max Occupancy({r.MaxOccupancy})");
                        }

                        Console.Write("Which room would you like to update? ");
                        int selectedRoomId = int.Parse(Console.ReadLine());
                        Room selectedRoom = roomOptions.FirstOrDefault(r => r.Id == selectedRoomId);

                        Console.Write("New Name: ");
                        selectedRoom.Name = Console.ReadLine();

                        Console.Write("New Max Occupancy: ");
                        selectedRoom.MaxOccupancy = int.Parse(Console.ReadLine());

                        roomRepo.Update(selectedRoom);

                        Console.WriteLine("Room has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;












                    case ("Delete a room"):
                        // Fetch all roommates
                        List<Room> roomOptions1 = roomRepo.GetAll();
                        //show list of roommates + info:
                        foreach (Room r in roomOptions1)
                        {
                           
                            Console.WriteLine($"{r.Id} - {r.Name} Max Occupancy({r.MaxOccupancy})");
                        }

                        Console.Write("Which room would you like to delete? (Enter room id) ");
                        if (!int.TryParse(Console.ReadLine(), out int selectedRoomId1))
                        {
                            Console.WriteLine("Invalid room ID.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        Room selectedRoom1 = roomOptions1.FirstOrDefault(r => r.Id == selectedRoomId1);

                        if (selectedRoom1 == null)
                        {
                            Console.WriteLine("Room not found.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        roomRepo.Delete(selectedRoom1.Id);

                        Console.WriteLine("Room has been successfully deleted");


                        //alt: try catch block for FK handling 
                        //// Confirm deletion with the user
                        //Console.WriteLine($"Are you sure you want to delete the room: Id={selectedRoom1.Id}, Name={selectedRoom1.Name}, Max Occupancy={selectedRoom1.MaxOccupancy}? (y/n)");
                        //string confirmation = Console.ReadLine();
                        //if (confirmation.ToLower() != "y")
                        //{
                        //    Console.WriteLine("Deletion canceled.");
                        //    Console.Write("Press any key to continue");
                        //    Console.ReadKey();
                        //    break;
                        //}

                        //try
                        //{
                        //    roomRepo.Delete(selectedRoom1.Id);
                        //    Console.WriteLine("Room has been successfully deleted.");
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine($"Error: {ex.Message}");
                        //}


                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;


















                    //first show list of all rooms and ask to select which to update.
                    //It then collects what they'd like the updated Name and Max Occupancy to be, and finally saves those changes to the database.
                    case ("Update a chore"):
                        List<Chore> choreOptions = choreRepo.GetAll();
                        foreach (Chore c in choreOptions)
                        {

                            Console.WriteLine($"{c.Id} - {c.Name} ");
                        }

                        Console.Write("Which chore would you like to update? ");
                        int selectedChoreId = int.Parse(Console.ReadLine());
                        Chore selectedChore = choreOptions.FirstOrDefault(c => c.Id == selectedChoreId);

                        Console.Write("New Name: ");
                        selectedChore.Name = Console.ReadLine();



                        choreRepo.Update(selectedChore);

                        Console.WriteLine("Chore has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;












                    case ("Delete a chore"):
                        // Fetch all chores
                        List<Chore> choreOptions1 = choreRepo.GetAll();
                        //show list of roommates + info:
                        foreach (Chore c in choreOptions1)
                        {

                            Console.WriteLine($"{c.Id} - {c.Name} ");
                        }

                        Console.Write("Which chore would you like to delete? (Enter chore id"); 
                        if (!int.TryParse(Console.ReadLine(), out int selectedChoreId1))
                        {
                            Console.WriteLine("Invalid chore");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        Chore selectedChore1 = choreOptions1.FirstOrDefault(c => c.Id == selectedChoreId1);

                        if (selectedChore1 == null)
                        {
                            Console.WriteLine("Chore not found.");
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }

                        choreRepo.Delete(selectedChore1.Id);

                        Console.WriteLine("Chore has been successfully deleted");

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;




                    case ("Exit"):
                        runProgram = false;
                        break;
                }
            }

        }

        static string GetMenuSelection()
        {
            Console.Clear();

            List<string> options = new List<string>()
            {
                "Show all rooms",
                "Search for room",
                "Add a room",
                "Show all chores",
                "Search for chore",
                "Add a chore",
                "Show unassigned chores",
                "Search for roommate",
                "Show all roommates",
                "Search for roommates using room Id",
                "Add a roommate",
                "Update roommate",
                "Delete roommate",
                "Update a room",
                "Delete a room",
                "Update a chore",
                "Delete a chore",
                "Exit"
            };

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Select an option > ");

                    string input = Console.ReadLine();
                    int index = int.Parse(input) - 1;
                    return options[index];
                }
                catch (Exception)
                {

                    continue;
                }
            }
        }
    }
}













