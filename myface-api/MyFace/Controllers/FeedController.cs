using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MyFace.Models.Database;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("feed")]
    public class FeedController : ControllerBase
    {
        private readonly IPostsRepo _posts;
        private readonly IUsersRepo _users;

        public FeedController(IPostsRepo posts, IUsersRepo users)
        {
            _posts = posts;
            _users = users;
        }

        [HttpGet("")]
        public ActionResult<FeedModel> GetFeed(
            [FromQuery] FeedSearchRequest searchRequest,
            [FromHeader] string authorization
            )
        {
            string encodedUsernamePassword = authorization.Substring("Basic ".Length);
            string usernamePassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
            string[] usernamePasswordArray = usernamePassword.Split(':');
            string username = usernamePasswordArray[0];
            string password = usernamePasswordArray[1];

            User user = _users.GetByUsername(username);
            
            


            var posts = _posts.SearchFeed(searchRequest);
            var postCount = _posts.Count(searchRequest);
            return FeedModel.Create(searchRequest, posts, postCount);
        }
    }
}
