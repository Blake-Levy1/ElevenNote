using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElevenNote.Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> RegisterUserAsync(UserRegister model)
        {
            if (await GetUserByEmailAsync(model.Email) != null || await GetUserByUsernameAsync(model.Username) != null)
            {
                return false;
            }
            var entity = new UserEntity
            {
                Email = model.Email,
                Username = model.Username,
                DateCreated = DateTime.Now
            };

            var passwordHasher = new PasswordHasher<UserEntity>();

            entity.Password = passwordHasher.HashPassword(entity, model.Password);

            _context.Users.Add(entity);
            var numberOfChanges = await _context.SaveChangesAsync();

            return numberOfChanges == 1;
        }

        public async Task<UserDetail> GetUserAsync(int userId)
        {
            var entity = await _context.Users.FindAsync(userId);
            // FindAsync (instead of FirstOrDefaultAsync) allows us to directly plug in the given primary key 
            if (entity == null)
            {
                return null;
            }

            var userDetail = new UserDetail
            {
                MyProperty = entity.MyProperty,
                Email = entity.Email,
                Username = entity.Username,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
            };

            return userDetail;
        }

        private async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(User => User.Email.ToLower() == email.ToLower());
            //FirstOrDefault will return null if no user is found
        }
        private async Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(User => User.Username.ToLower() == username.ToLower());
        }
    }
}