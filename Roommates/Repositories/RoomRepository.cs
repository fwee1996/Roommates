using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Room data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class RoomRepository : BaseRepository
    {
        /// <summary>
        ///  When new RoomRepository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public RoomRepository(string connectionString) : base(connectionString) { }






        // ...We'll add some methods shortly...
        /// <summary>
        ///  Get a list of all Rooms in the database
        /// </summary>
        public List<Room> GetAll()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, Name, MaxOccupancy FROM Room";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the rooms we retrieve from the database.
                    List<Room> rooms = new List<Room>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        int maxOccupancyColumPosition = reader.GetOrdinal("MaxOccupancy");
                        int maxOccupancy = reader.GetInt32(maxOccupancyColumPosition);

                        // Now let's create a new room object using the data from the database.
                        Room room = new Room
                        {
                            Id = idValue,
                            Name = nameValue,
                            MaxOccupancy = maxOccupancy,
                        };

                        // ...and add that room object to our list.
                        rooms.Add(room);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of rooms who whomever called this method.
                    return rooms;
                }
            }
        }








        /// <summary>
        ///  Returns a single room with the given id.
        /// </summary>
        public Room GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name, MaxOccupancy FROM Room WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Room room = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        room = new Room
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy")),
                        };
                    }

                    reader.Close();

                    return room;
                }
            }
        }









        /// <summary>
        ///  Add a new room to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void Insert(Room room)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Room (Name, MaxOccupancy) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@name, @maxOccupancy)";
                    cmd.Parameters.AddWithValue("@name", room.Name);
                    cmd.Parameters.AddWithValue("@maxOccupancy", room.MaxOccupancy);
                    int id = (int)cmd.ExecuteScalar();

                    room.Id = id;
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }





        /// <summary>
        ///  Updates the room
        /// </summary>
        public void Update(Room room)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Room
                                    SET Name = @name,
                                        MaxOccupancy = @maxOccupancy
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", room.Name);
                    cmd.Parameters.AddWithValue("@maxOccupancy", room.MaxOccupancy);
                    cmd.Parameters.AddWithValue("@id", room.Id);

                    cmd.ExecuteNonQuery();
                    //ExecuteNonQuery--bcs UPDATE statement: tell database to update a row, don't return anything.
                    //vs ExecuteReader--SELECT statements tell database to send data back and we read through it.
                }
            }

        }





        /// <summary>
        ///  Delete the room with the given id
        /// </summary>
        ///  take an int id as a parameter and not return anything. DELETE statement on the database but No RETURN so use the ExecuteNonQuery method.
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // What do you think this code will do if there is a roommate in the room we're deleting???
                    // Check for Assigned Roommates: Before attempting to delete the room,
                    // the code checks if any roommates are still assigned to the room.
                    // If so, an error is thrown, preventing deletion.
                    cmd.CommandText = @"
                IF EXISTS (SELECT 1 FROM Roommate WHERE RoomId = @id)
                BEGIN
                    THROW 50000, 'Cannot delete room. Roommates are still assigned to this room.', 1;
                END
                ELSE
                BEGIN
                    DELETE FROM Room WHERE Id = @id;
                END";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }




    }
}































//using System.Collections.Generic;
//using Microsoft.Data.SqlClient;
//using System.Linq;
//using Roommates.Models;

//namespace Roommates.Repositories
//{
//    public class RoomRepository
//    {
//        private readonly string _connectionString;

//        public RoomRepository(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public List<Room> GetAll()
//        {
//            using (SqlConnection conn = new SqlConnection(_connectionString))
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = "SELECT Id, Name, MaxOccupancy FROM Room";
//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        List<Room> rooms = new List<Room>();
//                        while (reader.Read())
//                        {
//                            rooms.Add(new Room
//                            {
//                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                                Name = reader.GetString(reader.GetOrdinal("Name")),
//                                MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
//                            });
//                        }
//                        return rooms;
//                    }
//                }
//            }
//        }

//        public Room GetById(int id)
//        {
//            using (SqlConnection conn = new SqlConnection(_connectionString))
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = "SELECT Id, Name, MaxOccupancy FROM Room WHERE Id = @id";
//                    cmd.Parameters.AddWithValue("@id", id);
//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            return new Room
//                            {
//                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                                Name = reader.GetString(reader.GetOrdinal("Name")),
//                                MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
//                            };
//                        }
//                        return null; // changes here: Return null if not found
//                    }
//                }
//            }
//        }

//        public void Insert(Room room)
//        {
//            using (SqlConnection conn = new SqlConnection(_connectionString))
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = "INSERT INTO Room (Name, MaxOccupancy) OUTPUT INSERTED.Id VALUES (@name, @maxOccupancy)";
//                    cmd.Parameters.AddWithValue("@name", room.Name);
//                    cmd.Parameters.AddWithValue("@maxOccupancy", room.MaxOccupancy);
//                    room.Id = (int)cmd.ExecuteScalar();
//                }
//            }
//        }
//    }
//}
