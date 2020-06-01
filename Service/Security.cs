using Microsoft.Ajax.Utilities;
using My_Social_Media_Channel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace My_Social_Media_Channel.Service
{
    public static class Security
    {

        /// <summary>
        /// Check if token is valid or not
        /// </summary>
        /// <param name="db"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string getJWTToken(string value)
        {
            //it can be changes to get actual Jwt token
            value = value + DateTime.Now.ToString();
            return getHashKey(value);
        }

        public static string getHashKey(string value)
        {
            using(SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
            //48475533050f1a0b7c6f3ff1aa75f5892a73d20a2d96a265f7041f4b36c3a425
        
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}