using System;
using Refugio.Models;
using Newtonsoft.Json;

namespace Refugio.Common.Clusterization
{
    public class SetMetrics
    {
        private Dictionary<string, int> Metrics = new Dictionary<string, int>();

        public List<int> SetUniversitiesToNumbers(List<User> users)
        {
            var res = new List<int>();
            this.SetDictionary("Univerties.json", NameOfUniversities.Universities);
            foreach (var user in users)
            {
                if (user.University != null)
                    if (Metrics.ContainsKey(user.University))
                        res.Add(Metrics[user.University]);
                    else
                        res.Add(7);
            }
            return res;
        }

        public List<int> SetFacultiesToNumbers(List<User> users)
        {
            var res = new List<int>();
            this.SetDictionary("Faculties.json", NameOfFaculties.Faculties);
            foreach (var user in users)
            {
                if (user.FacultyName != null)
                    if (Metrics.ContainsKey(user.FacultyName))
                        res.Add(Metrics[user.FacultyName]);
                    else
                        res.Add(9);
            }
            return res;
        }

        void SetDictionary(string path, List<string> metrics)
        {
            using (StreamReader r = new StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);

                for (var i = 0; i < metrics.Count(); i++)
                    Metrics = this.AssigningValues(array[metrics[i]], Metrics, i);
            }
        }

        public Dictionary<string, int> AssigningValues(dynamic array, Dictionary<string, int> dic, int i)
        {
            foreach (var z in array)
                dic.Add(z.ToString(), i);

            return dic;
        }
    }
}