using System;
using User = Refugio.Models.User;
using Group = Refugio.Models.Group;
using UserDB = Refugio.DataBase.Models.Models.User;
using GroupDB = Refugio.DataBase.Models.Models.Group;

namespace Refugio.Common
{
    public interface IUserRepository
    {
        Task CreateUser(User user);

        Task AddUser(User user);

        Task DeleteUser(long Id);

        Task<List<Group>?> GetListOfGroups(long Id);

        Task<int> CountOfUsers();

        Task<List<User>?> GetUsers();

        Task UpdateUser(UserDB newUser, UserDB oldUser);

        Task UpdateActivitiesUser(User user);

        Task<List<string?>> GetUsersBirthdays();

        Task<List<string?>> GetUserCities();

        Task<List<string?>> GetUserUniversities();

        Task<List<string?>> GetUserFaculties();
    }
}