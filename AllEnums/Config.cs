using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My_Social_Media_Channel.AllEnums
{
    public static class Config
    {
        public const string userTable = "User";
        public const string userPost = "Post";

        public static Guid userId;
        public static string userName = "";
        public static string Email = "";
        public static string Token = "";
        public const int TokenExpiresInMinutes = 15;
    }
}