using System;
using AngleSharp;
using VkNet.Model;
using Refugio.Models;
using VKAPI.Exceptions;
using VkNet.Enums.Filters;
using Newtonsoft.Json.Linq;
using User = Refugio.Models.User;
using Group = Refugio.Models.Group;

namespace VKAPI
{
    public class VkApiHandler : IVkApiHandler
    {
        private VkNet.VkApi apiVk;

        private string? GroupId_TypicalMSTU;

        private ulong ApplicationId;

        private string? Token;

        private ulong stepByGroupSubscribers = 1000;

        public VkApiHandler(string groupId,
                            ulong applicationId,
                            string token)
        {
            GroupId_TypicalMSTU = groupId;

            ApplicationId = applicationId;

            Token = token;

            apiVk = AuthorizationInVk();
        }

        private VkNet.VkApi AuthorizationInVk()
        {
            try
            {
                var apiVk = new VkNet.VkApi();

                apiVk.Authorize(new ApiAuthParams
                {
                    ApplicationId = ApplicationId,
                    AccessToken = Token,
                    Settings = Settings.All
                });

                return apiVk;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<VkNet.Model.User> GetListOfSubscribersOfTheGroupInVk()
        {
            try
            {
                var numberOfSubscribersOfTheGroup = this.apiVk.Groups.GetMembers(new GroupsGetMembersParams()
                {
                    GroupId = GroupId_TypicalMSTU,
                }).TotalCount;

                var listOfUserIds = new List<long>();

                for (ulong i = 0; i < numberOfSubscribersOfTheGroup; i += this.stepByGroupSubscribers)
                {
                    var subscribersOfTheGroup = apiVk.Groups.GetMembers(new GroupsGetMembersParams()
                    {
                        GroupId = GroupId_TypicalMSTU,
                        Offset = (long?)i
                    });

                    listOfUserIds.AddRange((IEnumerable<long>)subscribersOfTheGroup.Select(x => x.Id).ToList());
                }

                var subscribers = apiVk.Users.Get(listOfUserIds.Take((int)this.stepByGroupSubscribers))
                                                               .Where(x => x.IsClosed == false &&
                                                                           x.Deactivated == VkNet.Enums.StringEnums.Deactivated.Activated)
                                                               .ToList();

                for (ulong i = this.stepByGroupSubscribers;
                     i < numberOfSubscribersOfTheGroup;
                     i += this.stepByGroupSubscribers)
                {
                    subscribers.AddRange(apiVk.Users.Get(listOfUserIds.Skip(Convert.ToInt32(i))
                                                    .Take((int)this.stepByGroupSubscribers))
                                                    .Where(x => x.IsClosed == false &&
                                                                x.Deactivated == VkNet.Enums.StringEnums.Deactivated.Activated)
                                                    .ToList());
                }

                return subscribers;
            }
            catch (RequestException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<Group>? GetUserSubscriptionInformationById(string userId)
        {
            try
            {
                var us = apiVk.Users.Get(new List<long>() { (long)Convert.ToInt32(userId) });

                if (us[0].IsClosed == true ||
                    us[0].Deactivated != VkNet.Enums.StringEnums.Deactivated.Activated)
                    return null;

                var userSubscriptions = apiVk.Users.GetSubscriptions(Convert.ToInt32(userId), 50);

                var groups = apiVk.Groups.GetById((IEnumerable<string>)userSubscriptions
                                         .Select(x => x.Id.ToString()).Take(500).ToList(), null, GroupsFields.All)
                                         .Where(x => x.IsClosed == VkNet.Enums.GroupPublicity.Public &&
                                                     x.Deactivated == VkNet.Enums.StringEnums.Deactivated.Activated)
                                         .Select(x => x.Id.ToString()).ToList();

                var groupInformation = new List<Group>();

                foreach (var group in groups)
                {
                    var address = "https://api.vk.com/method/groups.getById?group_id=" +
                                  $"{group}" +
                                  "&fields=name,type,activity,city,country,description,members_count,place&access_token=" +
                                  $"{Token}" +
                                  "&v=5.131";

                    var config = Configuration.Default.WithDefaultLoader();

                    var document = BrowsingContext.New(config).OpenAsync(address).Result;

                    var dataJson = document.Body.TextContent.ToString();

                    var json = JObject.Parse(dataJson);

                    var informationAboutGroup = json["response"].ToList()[0].Children();

                    groupInformation.Add(CreatingAGroupModel(informationAboutGroup));

                    Thread.Sleep(200);
                }

                return groupInformation;
            }
            catch (AggregateException exception)
            {
                throw exception;
            }
            catch (RequestException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public Group CreatingAGroupModel(JEnumerable<JToken> informationAboutGroup)
        {
            var group = new Group();

            try
            {
                foreach (var inf in informationAboutGroup)
                {
                    if (inf.ToString().Contains("name"))
                    {
                        group.Name = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("type"))
                    {
                        group.Type = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("description"))
                    {
                        group.Description = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("members_count"))
                    {
                        group.MembersCount = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("activity"))
                    {
                        group.Activity = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("place"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        group.Place = _dataJson["title"].ToString();
                    }
                    else if (inf.ToString().Contains("city"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        group.City = _dataJson["title"].ToString();
                    }
                    else if (inf.ToString().Contains("country"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        group.Country = _dataJson["title"].ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return group;
        }

        public List<User> GetInformationAboutGroupUsersById()
        {
            var userInformation = new List<User>();

            try
            {
                var subscribersOfTheGroup = this.GetListOfSubscribersOfTheGroupInVk()
                                                .Select(x => x.Id.ToString()).ToList();

                foreach (var groupSubscriber in subscribersOfTheGroup)
                {
                    userInformation.Add(GetUserInformationById(groupSubscriber));

                    Thread.Sleep(200);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return userInformation;
        }

        public User GetUserInformationById(string userId)
        {
            try
            {
                var address = "https://api.vk.com/method/users.get?user_ids=" +
                              $"{userId}" +
                              "&fields=uid,first_name,last_name,nickname,sex,bdate,city,country,education&access_token=" +
                              $"{Token}" +
                              "&v=5.131";

                var config = Configuration.Default.WithDefaultLoader();

                var document = BrowsingContext.New(config).OpenAsync(address).Result;

                var dataJson = document.Body.TextContent.ToString();

                var json = JObject.Parse(dataJson);

                var informationAboutUser = json["response"].ToList()[0].Children();

                return CreatingAUserModel(informationAboutUser, userId);
            }
            catch (AggregateException exception)
            {
                throw exception;
            }
            catch (RequestException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public User CreatingAUserModel(JEnumerable<JToken> informationAboutUser, string usId)
        {
            var user = new User();

            try
            {
                int countStr = 0;

                foreach (var inf in informationAboutUser)
                {
                    if (inf.ToString().Contains("nickname"))
                    {
                        user.Nickname = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("id") && countStr == 0)
                    {
                        user.VkId = long.Parse(inf.First.ToString());
                    }
                    else if (inf.ToString().Contains("city"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        user.City = _dataJson["title"].ToString();
                    }
                    else if (inf.ToString().Contains("country"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        user.Country = _dataJson["title"].ToString();
                    }
                    else if (inf.ToString().Contains("university_name"))
                    {
                        user.University = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("faculty_name"))
                    {
                        user.FacultyName = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("sex"))
                    {
                        user.Sex = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("first_name"))
                    {
                        user.FirstName = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("last_name"))
                    {
                        user.LastName = inf.First.ToString();
                    }

                    countStr++;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return user;
        }
    }
}