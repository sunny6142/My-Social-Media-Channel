using My_Social_Media_Channel.AllEnums;
using My_Social_Media_Channel.Filters;
using My_Social_Media_Channel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace My_Social_Media_Channel.Controllers
{
    public class PostController : ApiController
    {
        Service.MongoDB db = new Service.MongoDB(Config.userPost);
        
        // GET: api/Post
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Post/5
        [ActionName("GetAllPost")]
        [System.Web.Http.HttpGet]
        public List<Post> GetAllPost()
        {
            return db.LoadRecord<Post>();
        }

        [ActionName("GetMyPost")]
        [System.Web.Http.HttpPost]
        [AuthGuard]
        public List<Post> GetMyPost()
        {
            return db.LoadRecordByCondition<Post>("UserId", Config.userId);
        }


        [System.Web.Http.HttpGet]
        [AuthGuard]
        public HttpResponseMessage GetPostById()
        {
            Response<Post> response = new Response<Post>();
            Post Post = new Post();
            string PostId = HttpContext.Current.Request.Params["PostId"];
            Post.Postid = Guid.Parse(PostId);//Guid.Parse(HttpContext.Current.Request.Form["PostId"]);

            Post = db.LoadRecordById<Post>(Post.Postid);

            response.Data =  Post;
            return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }

        // POST: api/Post
        [ActionName("AddPost")]
        [System.Web.Http.HttpPost]
        [AuthGuard]
        public HttpResponseMessage AddPost()
        {
            Response<String> response = new Response<String>();
            Post Post = new Post();
            try
            {
                Post.Message = HttpUtility.HtmlEncode(HttpContext.Current.Request.Form["Message"].ToLower());
            }
            catch(Exception ex)
            {
                response.Message = "Unacceptable Formate";
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }

            string fileName = HttpContext.Current.Request.Files["Image"].FileName.ToLower();
            if (!(fileName.Contains(".png") || fileName.Contains(".jpeg")))
            {

                response.Message = "Only Png and Jpeg File are allowed";
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            // Post.url = HttpContext.Current.Request.Form["url"];
            Post.UserId = Config.userId;
            Post.PostBy = Config.userName;
            Post.CreateOn = DateTime.Now;
            Post.ModifiedOn = DateTime.Now;

            using (var stream = HttpContext.Current.Request.Files["Image"].InputStream)
            {
                
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                Post.url = "data:image/png;base64," + base64String;
                Post.Image = bytes;
            }
            
            db.InsertRecord<Post>(Post);

            return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }

        // PUT: api/Post/5
        [ActionName("Update")]
        [System.Web.Http.HttpPut]
        [AuthGuard]
        public HttpResponseMessage Put()
        {
            Response<Post> response = new Response<Post>();
            Post Post = new Post();

            Post.Postid = Guid.Parse(HttpContext.Current.Request.Form["PostId"]);
            Post = db.LoadRecordById<Post>(Post.Postid);
            if (Post.UserId == Config.userId)
            {
                Post.Message = HttpContext.Current.Request.Form["Message"];
                Post.UserId = Config.userId;
                Post.PostBy = Config.userName;
                Post.ModifiedOn = DateTime.Now;
                using (var stream = HttpContext.Current.Request.Files["Image"].InputStream)
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    Post.url = "data:image/png;base64," + base64String;
                    Post.Image = bytes;
                }

                db.UpdateRecordById<Post>(Post.Postid, Post);
            }
            else
            {
                response.Status = AllCodes.Unauthorized;
                response.Message = AllStatus.Unauthorized;
            }

            return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }

        // DELETE: api/Post/5
        [ActionName("DeletePost")]
        [System.Web.Http.HttpDelete]
        [AuthGuard]
        public HttpResponseMessage Delete(string postId)
        {
            Response<Post> response = new Response<Post>();
            Post Post = new Post();

            Post.Postid = Guid.Parse(postId);
            Post = db.LoadRecordById<Post>(Post.Postid);
            if (Post.UserId == Config.userId)
            {
                db.DeleteRecordById<Post>(Post.Postid, Post);
            }
            else
            {
                response.Status = AllCodes.Unauthorized;
                response.Message = AllStatus.Unauthorized;
            }

            return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }
    }
}
