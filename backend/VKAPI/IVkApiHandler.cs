using System;
using Newtonsoft.Json.Linq;
using User = Refugio.Models.User;
using Group = Refugio.Models.Group;

namespace VKAPI
{
	public interface IVkApiHandler
	{
        public List<VkNet.Model.User> GetListOfSubscribersOfTheGroupInVk();

        public List<Group>? GetUserSubscriptionInformationById(string userId);

        public Group CreatingAGroupModel(JEnumerable<JToken> informationAboutUser);

        public List<User> GetInformationAboutGroupUsersById();

        public User GetUserInformationById(string userId);

        public User CreatingAUserModel(JEnumerable<JToken> informationAboutUser, string usId);
    }
}