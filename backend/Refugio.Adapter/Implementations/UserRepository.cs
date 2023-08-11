using System;
using AutoMapper;
using Refugio.Common;
using Refugio.Models;
using Refugio.DataBase;
using User = Refugio.Models.User;
using Group = Refugio.Models.Group;
using Microsoft.EntityFrameworkCore;
using Refugio.DataBase.Models.Models;
using UserDB = Refugio.DataBase.Models.Models.User;
using GroupDB = Refugio.DataBase.Models.Models.Group;

namespace Refugio.Adapter.Implementations
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(Context context, IMapper mapper) : base(context, mapper) { }

        public async Task CreateUser(User user)
        {
            try
            {
                _context.Users.Add(_mapper.Map<UserDB>(user));
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task AddUser(User user)
        {
            try
            {
                var us = _context.Users.Where(x => x.VkId == user.VkId).FirstOrDefault();

                if (us == null)
                {
                    await CreateUser(user);
                }
                else
                {
                    await UpdateUser(_mapper.Map<UserDB>(user), us);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task DeleteUser(long Id)
        {
            try
            {
                var user = _context.Users.Where(x => x.Id == Id).FirstOrDefault();

                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<List<Group>?> GetListOfGroups(long Id)
        {
            try
            {
                var us = _context.Users.Where(u => u.Id == Id).FirstOrDefault();

                if (us == null)
                    throw new Exception("User with such id doesn't exist");

                return _mapper.Map<List<Group>>(us.Groups.ToList());
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Task<int> CountOfUsers()
        {
            try
            {
                return Task.FromResult(_context.Users.Count());
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Task<List<User>?> GetUsers()
        {
            try
            {
                return Task.FromResult<List<User>?>(_mapper.Map<List<User>>(_context.Users.Include(x => x.Groups).ToList()));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task UpdateUser(UserDB newUser, UserDB oldUser)
        {
            try
            {
                if (oldUser != null)
                {
                    if (oldUser.FirstName != newUser.FirstName)
                    {
                        oldUser.FirstName = newUser.FirstName;
                    }
                    if (oldUser.LastName != newUser.LastName)
                    {
                        oldUser.LastName = newUser.LastName;
                    }
                    if (oldUser.Activities != newUser.Activities)
                    {
                        oldUser.Activities = newUser.Activities;
                    }
                    if (oldUser.Country != newUser.Country)
                    {
                        oldUser.Country = newUser.Country;
                    }
                    if (oldUser.City != newUser.City)
                    {
                        oldUser.City = newUser.City;
                    }
                    if (oldUser.FacultyName != newUser.FacultyName)
                    {
                        oldUser.FacultyName = newUser.FacultyName;
                    }
                    if (oldUser.Nickname != newUser.Nickname)
                    {
                        oldUser.Nickname = newUser.Nickname;
                    }
                    if (oldUser.University != newUser.University)
                    {
                        oldUser.University = newUser.University;
                    }
                    if (oldUser.Sex != newUser.Sex)
                    {
                        oldUser.Sex = newUser.Sex;
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task UpdateActivitiesUser(User user)
        {
            try
            {
                var userDB = await _context.Users.FirstOrDefaultAsync(x => x.VkId == user.VkId);

                if (userDB != null)
                {
                    userDB.Activities = user.Activities;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Task<List<string?>> GetUsersBirthdays()
        {
            try
            {
                return Task.FromResult(_context.Users.Where(x => x.BirthDate != null).Select(x => x.BirthDate).Distinct().ToList());
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Task<List<string?>> GetUserCities()
        {
            try
            {
                return Task.FromResult(_context.Users.Where(x => x.City != null).Select(x => x.City).Distinct().ToList());
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Task<List<string?>> GetUserUniversities()
        {
            try
            {
                return Task.FromResult(_context.Users.Where(x => x.University != null).Select(x => x.University).Distinct().ToList());
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Task<List<string?>> GetUserFaculties()
        {
            try
            {
                return Task.FromResult(_context.Users.Where(x => x.FacultyName != null).Select(x => x.FacultyName).Distinct().ToList());
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}