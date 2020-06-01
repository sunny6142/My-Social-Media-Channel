
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace My_Social_Media_Channel.Models
{
    public class Post
    {
        [BsonId]
        public Guid Postid { get; set; }
        public Guid UserId { get; set; }
        public string PostBy { get; set; }
        public string Message { get; set; }
        public byte[] Image { get; set; }
        public string url { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}