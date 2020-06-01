using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserClass>> GetUsers();
        Task<UserClass> AddUser(UserClass user);
        Task<UserClass> GetUser(int userId);
        Task<UserClass> UpdateUser(UserClass user);
        Task<UserClass> DeleteUser(int userId);
    }
}
