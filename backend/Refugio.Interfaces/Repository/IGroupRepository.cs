using System;
using Refugio.Models;
using User = Refugio.Models.User;
using Group = Refugio.Models.Group;
using UserDB = Refugio.DataBase.Models.Models.User;
using GroupDB = Refugio.DataBase.Models.Models.Group;

namespace Refugio.Interfaces.Repository
{
    public interface IGroupRepository
    {
        Task CreateGroup(Group group, User user);

        Task AddGroup(Group group, User user);

        Task ClearConnection();

        Task<List<string?>> GetAllActivity();

        Task UpdateGroup(GroupDB oldGroup, GroupDB newGroup);
    }
}