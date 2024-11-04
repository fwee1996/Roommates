using System;

namespace Roommates.Models
{
    // C# representation of the Roommate table
    public class Roommate
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int RentPortion { get; set; }
        public DateTime MoveInDate { get; set; }
        public Room Room { get; set; }
        //public string? RoomName { get; set; }  // Added this property to hold just the room name
    }
}