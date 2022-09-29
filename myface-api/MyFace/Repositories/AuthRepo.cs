using System.Collections.Generic;
using System.Linq;
using MyFace.Helpers;
using MyFace.Models.Database;
using MyFace.Models.Request;


namespace MyFace.Repositories
{
    public interface IAuthRepo
    {
        bool IsAuthorised(string username, string password);
    }


    public class AuthRepo : IAuthRepo
    {
        private readonly IPostsRepo _posts;
        private readonly IUsersRepo _users;
        private readonly MyFaceDbContext _context;
        // public AuthRepo(MyFaceDbContext context)
        // {
        //     _context = context;
        // }

        public AuthRepo(IPostsRepo posts, IUsersRepo users)
        {
            _posts = posts;
            _users = users;
        }
        public bool IsAuthorised(string username, string password)
        {
            User user = _users.GetByUsername(username);

            byte[] salt = user.Salt;
            string hash = PasswordHelper.HashGenerator(password, salt);


            if (hash != user.HashedPassword)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}