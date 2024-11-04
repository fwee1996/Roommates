using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    public class ChoreRepository : BaseRepository
    {

        public ChoreRepository(string connectionString) : base(connectionString) { }

        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = "SELECT Id, Name FROM Chore";


                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Chore> chores = new List<Chore>();
                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");


                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);



                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue,
                            
                        };


                        chores.Add(chore);
                    }
                    reader.Close();

                    return chores;
                }
            }
        }

        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id"; //need to use join statement for roommate check for chore DELETE method. Cant delete chore if theres roommate assigned

                    //Add JOIN in SQL Query:
                    cmd.CommandText = @"
                    SELECT 
                        c.Id AS ChoreId, 
                        c.Name, 
                        rc.RoommateId AS RoommateId, 
                        rc.ChoreId AS ChoreId, 

                    FROM  Chore c

                    JOIN RoommateChore rc ON c.Id = rc.ChoreId

                    WHERE c.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Chore chore = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        chore = new Chore
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                           
                        };
                    }

                    reader.Close();

                    return chore;
                }
            }
        }

        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Inserts a new row called Name into the Chore table with the @Name parameter's value and returns the auto-generated Id of the new row.
                    cmd.CommandText = @"INSERT INTO Chore (Name) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);

                    // Execute the command and get the newly inserted tag ID
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;// Assign the new ID to the chore object
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }


        //Add a method to ChoreRepository called GetUnassignedChores.
        ////It should not accept any parameters and should return a list of chores that don't have any roommates already assigned to them. After implementing this method, add an option to the menu so the user can see the list of unassigned chores.
        public List<Chore> GetUnassignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Need LEFT JOIN bcs you WANT NULL case
                    cmd.CommandText = "SELECT c.Id, c.Name FROM Chore  c LEFT JOIN RoommateChore rc ON rc.ChoreId = c.Id WHERE rc.ChoreId IS NULL";


                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Chore> unassignedChores = new List<Chore>();
                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");


                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);



                        
                        Chore unassignedChore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue,

                        };

                       


                        unassignedChores.Add(unassignedChore);
                    }
                    reader.Close();

                    return unassignedChores;
                }
            }
        }




        //Update the ChoreRepository to include an Update and Delete method and give the user menu options to be able to use this functionality. NOTE: What happens when a user tries to delete a Chore that's been assigned to someone? Why does this happen? What can/should be done about this?


        /// <summary>
        ///  Updates the chore
        /// </summary>
        public void Update(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Chore
                                    SET Name = @name 
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", chore.Name); //before WHERE NO COMMA after @Name! Otherwise:'Incorrect syntax near the keyword 'WHERE'.'
                    cmd.Parameters.AddWithValue("@id", chore.Id);

                    cmd.ExecuteNonQuery();
                    //ExecuteNonQuery--bcs UPDATE statement: tell database to update a row, don't return anything.
                    //vs ExecuteReader--SELECT statements tell database to send data back and we read through it.
                }
            }

        }






        /// <summary>
        ///  Delete the chore with the given id
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
                IF EXISTS (SELECT 1 FROM RoommateChore WHERE ChoreId = @id) 
                BEGIN
                    THROW 50000, 'Cannot delete chore. Roommates are still assigned to this chore.', 1;
                END
                ELSE
                BEGIN
                    DELETE FROM Chore WHERE Id = @id;
                END";
                    //// Add parameter value to prevent SQL injection and to safely pass the ID
                    cmd.Parameters.AddWithValue("@id", id);

                    // Execute the command
                    cmd.ExecuteNonQuery();
                }
            }
        }






    }
}




