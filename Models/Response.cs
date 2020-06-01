
using System;
using System.Collections.Generic;
using My_Social_Media_Channel.AllEnums;

namespace My_Social_Media_Channel.Models
{
    public class Response<T>
    {
        public string AccessToken { get; set; }
        public T Data { get; set; }
        public string Message { get; set; } 
        public int Status { get; set; } 
        public Response()
        {
            Message = AllStatus.Success;
            Status = AllCodes.Success;
        }
    }
}