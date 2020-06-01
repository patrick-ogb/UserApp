using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<UserClass> AddUser(UserClass user)
        {
            context.UsersClass.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<UserClass> DeleteUser(int userId)
        {
            var result = context.UsersClass.FirstOrDefault(u => u.Id == userId);
            if (result != null)
            {
                context.Remove(result);
                await context.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<UserClass> GetUser(int Id)
        {
            return await context.UsersClass.FirstOrDefaultAsync(u => u.Id == Id);
        }

        public async Task<IEnumerable<UserClass>> GetUsers()
        {
            return await context.UsersClass.ToListAsync();
        }

        public async Task<UserClass> UpdateUser(UserClass peopleChange)
        {
            var people = context.UsersClass.Attach(peopleChange);
            people.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();

            return peopleChange;
        }
    }
}
