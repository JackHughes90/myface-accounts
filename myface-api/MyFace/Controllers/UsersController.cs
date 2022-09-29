using Microsoft.AspNetCore.Mvc;
using MyFace.Helpers;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _users;
        private readonly IAuthRepo _auth;

        public UsersController(IUsersRepo users, IAuthRepo auth)
        {
            _users = users;
            _auth = auth;
        }
        
        [HttpGet("")]
        public ActionResult<UserListResponse> Search([FromQuery] UserSearchRequest searchRequest, 
                                                     [FromHeader] string authorization)
        {
            var usernamePassword = PasswordHelper.DecodeAuthHeader(authorization);
            string username = usernamePassword.Username;
            string password = usernamePassword.Password;
            
            if (!_auth.IsAuthorised(username, password))
            {
                return Unauthorized();
            }
            var users = _users.Search(searchRequest);
            var userCount = _users.Count(searchRequest);
            return UserListResponse.Create(searchRequest, users, userCount);
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var user = _users.GetById(id);
            return new UserResponse(user);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = _users.Create(newUser);

            var url = Url.Action("GetById", new { id = user.Id });
            var responseViewModel = new UserResponse(user);
            return Created(url, responseViewModel);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.Update(id, update);
            return new UserResponse(user);
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _users.Delete(id);
            return Ok();
        }
    }
}