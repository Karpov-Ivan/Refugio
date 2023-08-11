using System;

namespace Refugio.Common.Clusterization
{
    public class CombiningInterests
    {
        public List<string> Combine(List<string> metrics, Dictionary<string, int> dict)
        {
            List<string> result = new List<string>();

            foreach (var act in metrics)
            {
                foreach (var dic in dict)
                {
                    if (act == dic.Key)
                        result.Add(NameOfInterests.Interests[dic.Value]);
                }
            }

            return result;
        }
    }
}