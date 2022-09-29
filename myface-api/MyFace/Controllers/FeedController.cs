using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MyFace.Models.Database;
using MyFace.Helpers;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("feed")]
    public class FeedController : ControllerBase
    {
        private readonly IPostsRepo _posts;
    
        private readonly IAuthRepo _auth;


        public FeedController(IPostsRepo posts, IAuthRepo auth)
        {
            _posts = posts;
            _auth = auth;
        }

        [HttpGet("")]
        public ActionResult<FeedModel> GetFeed(
            [FromQuery] FeedSearchRequest searchRequest,
            [FromHeader] string authorization
            )
        {
            var usernamePassword = PasswordHelper.DecodeAuthHeader(authorization);
            string username = usernamePassword.Username;
            string password = usernamePassword.Password;
            
            if (!_auth.IsAuthorised(username, password))
            {
                return Unauthorized();
            }

            var posts = _posts.SearchFeed(searchRequest);
            var postCount = _posts.Count(searchRequest);
            return FeedModel.Create(searchRequest, posts, postCount);
        }
    }
}
