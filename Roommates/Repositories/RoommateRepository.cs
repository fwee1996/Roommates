//Create a RoommateRepository and implement only the GetById method. It should take in a int id as a parameter and return a Roommate object. The trick: When you add a menu option for searching for a roommate by their Id, the output to the screen should output their first name, their rent portion, and the name of the room they occupy. Hint: You'll want to use a JOIN statement in your SQL query

using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Room data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class RoommateRepository : BaseRepository //inheriting from BaseRepo
    {
        /// <summary>
        ///  When new RoommateRepository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public RoommateRepository(string connectionString) : base(connectionString) { } //looking for connection string


        ///// <summary>
        /////  Returns a single room with the given id.
        ///// </summary>
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //cmd.CommandText = "SELECT FirstName, LastName, RentPortion, MoveInDate, Room FROM Roommate WHERE Id = @id";

                    //Add JOIN in SQL Query:
                    cmd.CommandText = @"
                    SELECT 
                        r.Id AS RoommateId, 
                        r.FirstName, 
                        r.LastName, 
                        r.RentPortion, 
                        r.MoveInDate, 
                        ro.Id AS RoomId, 
                        ro.[Name] AS RoomName, 
                        ro.MaxOccupancy 

                    FROM Roommate r

                    JOIN Room ro ON r.RoomId = ro.Id

                    WHERE r.Id = @id";

                    //if you leave Roommate prop :  public Room Room { get; set; } its whole object Room so need to have all Room obj props in SELECT:
                    ////ro.Id AS RoomId, 
                    //ro.[Name] AS RoomName, 
                    //ro.MaxOccupancy 



                    //alt:ro.[Name] AS RoomName



                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        // Create the Room object
                        Room room = new Room
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                            Name = reader.GetString(reader.GetOrdinal("RoomName")),
                            MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
                        };

                        // Create the Roommate object
                        roommate = new Roommate
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MoveInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = room, //Assign the manually created Room object

                        };
                    }

                    reader.Close();

                    return roommate;
                }

            }
        }




        //public List<Roommate> GetAll()
        //Roommate objects should have a null value for their Room property
        public List<Roommate> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT 
                        r.Id AS RoommateId, 
                        r.FirstName, 
                        r.LastName, 
                        r.RentPortion, 
                        r.MoveInDate, 
                        ro.Id AS RoomId, 
                        ro.[Name] AS RoomName, 
                        ro.MaxOccupancy 

                    FROM Roommate r

                    LEFT JOIN Room ro ON r.RoomId = ro.Id"; //dont have Room in SELECT!


                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Roommate> roommates = new List<Roommate>();
                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("RoommateId");
                        int idValue = reader.GetInt32(idColumnPosition);

                        int firstNameColumnPosition = reader.GetOrdinal("FirstName");
                        string firstNameValue = reader.GetString(firstNameColumnPosition);

                        int lastNameColumnPosition = reader.GetOrdinal("LastName");
                        string lastNameValue = reader.GetString(lastNameColumnPosition);

                        int rentPortionColumnPosition = reader.GetOrdinal("RentPortion");
                        int rentPortionValue = reader.GetInt32(rentPortionColumnPosition);

                        int moveInDateColumnPosition = reader.GetOrdinal("MoveInDate");
                        DateTime moveInDateValue = reader.GetDateTime(moveInDateColumnPosition);

                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            FirstName = firstNameValue,
                            LastName = lastNameValue,
                            RentPortion = rentPortionValue,
                            MoveInDate = moveInDateValue,
                            Room = new Room //dont have Room room =  
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                                Name = reader.GetString(reader.GetOrdinal("RoomName")),
                                MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
                            }//you had Room=null; initially but for #12 Update Roommate you use GetAll and you need roomId for each rooomate that will not be null! so dont keep room null here.  

                        };

                        roommates.Add(roommate);
                    }
                    reader.Close();

                    return roommates;
                }
            }
        }



        ///// <summary>
        /////  Returns list of roommates with the given room id.
        ///// </summary>
        //public List<Roommate> GetRoommatesByRoomId(int roomId)
        //Roommate objects should have a Room property
        public List<Roommate> GetRoommatesByRoomId(int roomId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    //Add JOIN in SQL Query:
                    cmd.CommandText = @"
                    SELECT 
                        r.Id AS RoommateId, 
                        r.FirstName, 
                        r.LastName, 
                        r.RentPortion, 
                        r.MoveInDate, 
                        ro.Id AS RoomId, 
                        ro.[Name] AS RoomName, 
                        ro.MaxOccupancy 

                    FROM Roommate r

                    LEFT JOIN Room ro ON r.RoomId = ro.Id

                    WHERE ro.Id = @id"; //make roomId the @id

                    // Add parameter to SQL query to prevent SQL injection:
                    cmd.Parameters.AddWithValue("@id", roomId); //roomId from parameter in method declaration line above
                    SqlDataReader reader = cmd.ExecuteReader();

                    //Roommate roommate = null;
                    ////Room room = null;
                    List<Roommate> roommates = new List<Roommate>();


                    // If we only expect a single row back from the database, we don't need a while loop.
                    //    if (reader.Read())
                    //{


                    //Need while loop because multiple roommates:
                    //iterate through all the rows returned by the query.
                    // Iterate through the results to create a list of Roommate objects
                    while (reader.Read())
                    {

                        // Create the Room object
                        Room room = new Room
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                            Name = reader.GetString(reader.GetOrdinal("RoomName")),
                            MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
                        };

                        // Create the Roommate object
                        Roommate roommate = new Roommate
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("RoommateId")), // Changed to RoommateId,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MoveInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = room, //Assign the manually created Room object

                        };
                        roommates.Add(roommate); // Add the Roommate object to the list
                    }

                    //}

                    reader.Close();

                    return roommates; // Return the list of Roommates
                }

            }
        }




        //public void Insert(Roommate roommate)
        public void Insert(Roommate roommate)
        {
            using (SqlConnection conn = Connection)// Create a connection to the database
            {
                {
                    conn.Open();// Open the connection
                    using (SqlCommand cmd = conn.CreateCommand())// Create a new command object
                    {
                        // SQL query to insert a new Roommate into the Roommate table
                        cmd.CommandText = @"
                        INSERT INTO Roommate (FirstName, LastName, RentPortion, MoveInDate, RoomId) 
                        OUTPUT INSERTED.Id 
                        VALUES (@FirstName, @LastName, @RentPortion, @MoveInDate, @RoomId)"; // RoomId is taken from the Room object

                        // Add parameters to the SQL query to prevent SQL injection
                        cmd.Parameters.AddWithValue("@FirstName", roommate.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", roommate.LastName);
                        cmd.Parameters.AddWithValue("@RentPortion", roommate.RentPortion);
                        cmd.Parameters.AddWithValue("@MoveInDate", roommate.MoveInDate);
                        cmd.Parameters.AddWithValue("@RoomId", roommate.Room.Id); // NOTE: RoomId is taken from the Room object

                        // Execute the query and retrieve the new Roommate's Id
                        int id = (int)cmd.ExecuteScalar();

                        // Set the Id property of the Roommate object to the new Id
                        roommate.Id = id;
                    }
                }

                // when this method is finished we can look in the database and see the new room.
            }
        }


        public void Update(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                UPDATE Roommate
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    RentPortion = @RentPortion,
                    MoveInDate = @MoveInDate,
                    RoomId = @RoomId
                WHERE Id = @Id";


                    cmd.Parameters.AddWithValue("@FirstName", roommate.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", roommate.LastName);
                    cmd.Parameters.AddWithValue("@RentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@MoveInDate", roommate.MoveInDate);
                    cmd.Parameters.AddWithValue("@RoomId", roommate.Room.Id);
                    cmd.Parameters.AddWithValue("@Id", roommate.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }






        //public void Delete(int id)
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                DELETE Roommate
                WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
















