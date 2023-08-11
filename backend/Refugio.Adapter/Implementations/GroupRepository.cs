using System;
using AutoMapper;
using Refugio.DataBase;
using User = Refugio.Models.User;
using Group = Refugio.Models.Group;
using Refugio.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using UserDB = Refugio.DataBase.Models.Models.User;
using GroupDB = Refugio.DataBase.Models.Models.Group;

namespace Refugio.Adapter.Implementations
{
    public class GroupRepository : BaseRepository, IGroupRepository
    {

        public GroupRepository(Context context, IMapper mapper) : base(context, mapper) { }

        public async Task CreateGroup(Group group, User user)
        {
            try
            {
                var groupDB = _mapper.Map<GroupDB>(group);
                _context.Groups.Add(groupDB);
                await _context.SaveChangesAsync();


                var userDB = await _context.Users.FirstOrDefaultAsync(x => x.VkId == user.VkId);
                groupDB.Users.Add(userDB);
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Task ClearConnection()
        {
            try
            {
                if (_context.Groups.Count() > 0)
                {
                    foreach (var group in _context.Groups)
                    {
                        if (group.Users != null)
                        {
                            group.Users.Clear();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return Task.CompletedTask;
        }

        public async Task AddGroup(Group group, User user)
        {
            try
            {
                var groupDB = _context.Groups.Include(x => x.Users).FirstOrDefault(s => s.Name == group.Name);

                if (groupDB == null)
                {
                    await CreateGroup(group, user);
                }
                else
                {
                    var userDB = await _context.Users.FirstOrDefaultAsync(x => x.VkId == user.VkId);

                    if (userDB != null && groupDB.Users != null)
                    {
                        groupDB.Users.Remove(userDB);
                    }

                    groupDB.Users.Add(userDB);
                    await UpdateGroup(groupDB, _mapper.Map<GroupDB>(group));
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Task<List<string?>> GetAllActivity()
        {
            return Task.FromResult(_context.Groups.Where(x => x.Activity != null &&
                                                              x.Activity.Contains(":") == false &&
                                                              x.Activity.Contains("Этот материал заблокирован") == false)
                                                  .Select(x => x.Activity).Distinct().ToList());
        }

        public async Task UpdateGroup(GroupDB oldGroup, GroupDB newGroup)
        {
            try
            {
                if (oldGroup.IsClosed != newGroup.IsClosed)
                {
                    oldGroup.IsClosed = newGroup.IsClosed;
                }
                if (oldGroup.Activity != newGroup.Activity)
                {
                    oldGroup.Activity = newGroup.Activity;
                }
                if (oldGroup.Description != newGroup.Description)
                {
                    oldGroup.Description = newGroup.Description;
                }
                if (oldGroup.Place != newGroup.Place)
                {
                    oldGroup.Place = newGroup.Place;
                }
                if (oldGroup.Country != newGroup.Country)
                {
                    oldGroup.Country = newGroup.Country;
                }
                if (oldGroup.Type != newGroup.Type)
                {
                    oldGroup.Type = newGroup.Type;
                }
                if (oldGroup.City != newGroup.City)
                {
                    oldGroup.City = newGroup.City;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}