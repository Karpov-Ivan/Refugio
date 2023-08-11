using System;
using Refugio.Models;
using Newtonsoft.Json;

namespace Refugio.Common.Clusterization
{
    public class SetInterests
    {
        private Dictionary<string, int> activities = new Dictionary<string, int>();

        public void Activity()
        {
            this.CreatingDictionaryWithInterests();
        }

        public void CreatingDictionaryWithInterests()
        {
            using (StreamReader r = new StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BrokenActivity.json")))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);

                for (var i = 0; i < NameOfInterests.Interests.Count(); i++)
                    activities = this.AssigningValuesToInterests(array[NameOfInterests.Interests[i]],
                                                                 activities,
                                                                 i);
            }
        }

        public Dictionary<string, int> AssigningValuesToInterests(dynamic array,
                                                                  Dictionary<string, int> dic,
                                                                  int i)
        {
            foreach (var z in array)
                dic.Add(z.ToString(), i);

            return dic;
        }

        public void EstablishInterests(HashSet<User> users)
        {
            foreach (var user in users)
            {
                if (user.Activities != null)
                {
                    var activity = user.Activities.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
                    user.Activities = $"{NameOfInterests.Interests[activity[0]]}, {NameOfInterests.Interests[activity[1]]}, {NameOfInterests.Interests[activity[2]]}, {NameOfInterests.Interests[activity[3]]}";
                }
            }
        }

        public void SetActivities(List<User> users, IUserRepository userDb, CombiningInterests cmb)
        {
            foreach (User u in users)
            {
                var newAct = cmb.Combine(u.Groups.Select(x => x.Activity).ToList(), this.activities);
                var userActivities = newAct.GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
                if (userActivities.Count() >= 4)
                {
                    u.Activities = string.Concat(string.Concat($"{NameOfInterests.Interests.FindIndex(x => x == userActivities[0].Key).ToString()} ", $"{NameOfInterests.Interests.FindIndex(x => x == userActivities[1].Key).ToString()} "), string.Concat($"{NameOfInterests.Interests.FindIndex(x => x == userActivities[2].Key).ToString()} ", $"{NameOfInterests.Interests.FindIndex(x => x == userActivities[3].Key).ToString()}"));
                    userDb.UpdateActivitiesUser(u);
                }
            }
        }

        public User SetActivitiesForOneUser(User user, List<Group> groups, CombiningInterests cmb)
        {
            var newAct = cmb.Combine(groups.Select(x => x.Activity).ToList(), this.activities);
            var userActivities = newAct.GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
            if (userActivities.Count() >= 4)
            {
                user.Activities = string.Concat(string.Concat($"{NameOfInterests.Interests.FindIndex(x => x == userActivities[0].Key).ToString()} ", $"{NameOfInterests.Interests.FindIndex(x => x == userActivities[1].Key).ToString()} "), string.Concat($"{NameOfInterests.Interests.FindIndex(x => x == userActivities[2].Key).ToString()} ", $"{NameOfInterests.Interests.FindIndex(x => x == userActivities[3].Key).ToString()}"));
            }

            return user;
        }
    }
}