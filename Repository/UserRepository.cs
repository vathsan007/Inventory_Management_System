using WebapiProject.Models;

namespace WebapiProject.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;

        public UserRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        private readonly List<User> users = new List<User>();

        public int AddUser(User user)
        {
            if (user == null)
            {
                throw new System.Exception("User cannot be null.");
            }

            db.Users.Add(user);
            return db.SaveChanges();
        }

        public User GetUserById(int Id)
        {
            var user = db.Users.FirstOrDefault(x => x.UserId == Id);
            if (user == null)
            {
                throw new System.Exception("User not found.");
            }

            return user;
        }

        public User GetUserByUsername(string username)
        {
            return db.Users.FirstOrDefault(x => x.Username == username);
        }

        public bool ValidateUserCredentials(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                throw new System.Exception("Invalid username or password.");
            }

            return true;
        }
    }
}