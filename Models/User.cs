
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My_Social_Media_Channel.Models
{
    public class User
    {
        [BsonId]
        public Guid Userid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string DBToken { get; set; }
        public int tokenExpires { get; set; }
    }
}