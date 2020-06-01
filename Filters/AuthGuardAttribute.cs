using My_Social_Media_Channel.AllEnums;
using My_Social_Media_Channel.Models;
using My_Social_Media_Channel.Service;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace My_Social_Media_Channel.Filters
{
    public class AuthGuardAttribute : AuthorizationFilterAttribute
    {
        Service.MongoDB db = new Service.MongoDB(Config.userTable);
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                string authKey = HttpContext.Current.Request.Headers["Authorization"];
                if (actionContext.Request.Headers.Authorization == null && authKey == null)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    // Gets header parameters  
                    string originalString = authKey;// actionContext.Request.Headers.Authorization.Parameter;
                    //    string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));

                    // Gets email  
                    string email = originalString.Split('#')[1];

                    Config.Token = originalString;
                   

                    // Validate email and token  
                    if (!ValidateUser(email, originalString))
                    {
                        // returns unauthorized error  
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }

                base.OnAuthorization(actionContext);
            }
            // Handling Authorize: Basic <base64(username:password)> format.
            catch (Exception e)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }
        public bool ValidateUser(string email, string token)
        {
            User user = new User();
            
            user.Email = Security.Base64Decode(email);

            user = db.LoadUserInfo<User>(user);

            Config.Email = user.Email;
            Config.userName = user.FirstName + " " + user.LastName;
            Config.userId = user.Userid;

            string checkDBToken = token;// Security.getJWTToken(token);
            
            if (user.DBToken == checkDBToken)
            {
                int unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                if (unixTimestamp < user.tokenExpires)
                {
                    // Auth Success;
                    user.tokenExpires = (Int32)(DateTime.UtcNow.AddMinutes(Config.TokenExpiresInMinutes).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    db.UpdateRecordById<User>(user.Userid, user);
                    return true;
                }
                else
                {
                    //token Expied
                    return false;
                }
            }
            // Auth Failed
            return false;
        }
    
    }

}