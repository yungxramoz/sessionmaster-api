using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Helpers;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using System.Linq;

namespace SessionMaster.BLL.ModUser
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SessionMasterContext context) : base(context)
        {
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new InfoException("Username and password is required");
            }

            User user = Get(u => u.Username == username).FirstOrDefault();

            if (user == null)
            {
                // not allowed to give proper information what's wrong
                throw new InfoException("Username or password is incorrect");
            }

            if (!PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                // not allowed to give proper information what's wrong
                throw new InfoException("Username or password is incorrect");
            }

            return user;
        }

        public User Add(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InfoException("Password is required");
            }

            if (_context.Users.Any(u => u.Username == user.Username))
            {
                throw new InfoException("Username already taken");
            }

            byte[] passwordHash;
            byte[] passwordSalt;
            PasswordHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            Add(user);

            return user;
        }

        public User Update(User user, string password)
        {
            User updateUser = GetById(user.Id);

            if (updateUser == null)
            {
                throw new NotFoundException("User not found");
            }

            if (!string.IsNullOrWhiteSpace(user.Username) && user.Username != updateUser.Username)
            {
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    throw new InfoException("Username already taken");
                }

                updateUser.Username = user.Username;
            }

            if (!string.IsNullOrWhiteSpace(user.Firstname))
            {
                updateUser.Firstname = user.Firstname;
            }

            if (!string.IsNullOrWhiteSpace(user.Lastname))
            {
                updateUser.Lastname = user.Lastname;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash;
                byte[] passwordSalt;
                PasswordHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                updateUser.PasswordHash = passwordHash;
                updateUser.PasswordSalt = passwordSalt;
            }

            return Update(updateUser);
        }
    }
}