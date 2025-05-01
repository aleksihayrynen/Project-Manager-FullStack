﻿using MongoDB.Bson;

namespace ProjectManager.Models
{
    public class UserSearchList
    {
        public ObjectId UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }


    }
}
