using My_Social_Media_Channel.AllEnums;
using My_Social_Media_Channel.Filters;
using My_Social_Media_Channel.Models;
using My_Social_Media_Channel.Service;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace My_Social_Media_Channel.Controllers
{
    public class AuthController : ApiController
    {
        Service.MongoDB db = new Service.MongoDB(Config.userTable);

        /// <summary>
        /// Add new user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [ActionName("Register")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Register([FromBody]User user)
        {
            Response<String> response = new Response<String>();
            
            validateUserDate(user, response);
            
            if (response.Message == AllStatus.Success)
            {
                db.InsertRecord(user);
                response.Message = "You account is successfully created our account You Can Login Now";
            }

            return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }
        
        private void validateUserDate(User user, Response<String> response)
        {
            user.Email = HttpUtility.HtmlEncode(user.Email);
            user.Password = HttpUtility.HtmlEncode(user.Password);
            user.ConfirmPassword = HttpUtility.HtmlEncode(user.ConfirmPassword);
            user.FirstName = HttpUtility.HtmlEncode(user.FirstName);
            user.LastName = HttpUtility.HtmlEncode(user.LastName);

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match matchEmail = regex.Match(user.Email);

            regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
            Match matchPassword = regex.Match(user.Password);

            if (user.ConfirmPassword != user.Password)
            {
                //Password and Confirm Password does not match
                response.Message = "Password and Confirm Password does not match";
                return;
            }
            
            user.Password = Security.getHashKey(user.Password);

            if (!matchEmail.Success)
            {
                //invalid email Id
                response.Message = "Enter a valid Email Id";
                return;
            }
            
            if (!matchPassword.Success)
            {
                //password should have lowerCase + UpperCase + Number + SpecialChar
                response.Message = "password Should Contain LowerCase, UpperCase, Number and SpecialChar and in the range of 8 to 15 char";
                return;
            }
            
        }

        /// <summary>
        /// Login to get AccessToken
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [ActionName("Login")]
        [HttpPost]
        public HttpResponseMessage Login(User user)
        {
            Response<String> response = new Response<String>();

            User userInfo = db.LoadUserInfo<User>(user);

            if(userInfo.Password == Security.getHashKey(user.Password))
            {
                string userName = $"{userInfo.FirstName} {userInfo.LastName}";
                response.AccessToken = $"{Security.Base64Encode(userName)}#{Security.Base64Encode(userInfo.Email)}#{Security.getJWTToken(userInfo.Email)}";
                userInfo.DBToken = response.AccessToken;//Security.getJWTToken(response.AccessToken);
                userInfo.tokenExpires = (Int32)(DateTime.UtcNow.AddMinutes(Config.TokenExpiresInMinutes).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    
                db.UpdateRecordById(userInfo.Userid, userInfo);

            }
            else
            {
                response.Message = AllStatus.AuthenticationFailed;
            }

            return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }

        public HttpResponseMessage LogOut()
        {
            Response<String> response = new Response<String>();

            User user = new User();

            string authKey = HttpContext.Current.Request.Headers["Authorization"];

            user.Email = Security.Base64Decode(authKey.Split('#')[1]);

            User userInfo = db.LoadUserInfo<User>(user);

            user.DBToken = null;
            user.tokenExpires = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;// DateTime.Now.Millisecond;

            db.UpdateRecordById<User>(userInfo.Userid, userInfo);

            return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }

    }
}
