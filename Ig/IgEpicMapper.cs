using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace IgTrading.Ig
{
    public class IgEpicMapper
    {
        private static string path = "./Store/igmapper.json";
        private static string handMapped = "./Store/tickerToIgEpic.json";


        private static Dictionary<string, string> lookupDictionary = new Dictionary<string, string>();

        static IgEpicMapper()
        {
            if (File.Exists(path))
            {
                string fileContents = File.ReadAllText(path);
                lookupDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContents);
            }
        }

        public static bool TryLookupCode(string igEpic, out string ticker)
        {
            lock (string.Intern("dictionaryEpicLock"))
            {
                igEpic = GetEpicWithoutExpiry(igEpic);
                return lookupDictionary.TryGetValue(igEpic, out ticker);
            }
        }

        private static string GetEpicWithoutExpiry(string igEpic)
        {
            Regex regex = new Regex("(?<=\\.).[^.]*$");
            igEpic = regex.Replace(igEpic, string.Empty).Trim('.');
            igEpic = regex.Replace(igEpic, string.Empty).Trim('.');
            return igEpic;
        }

        public static void AddCode(string igEpic, string ticker)
        {
            lock (string.Intern("dictionaryEpicLock"))
            {
                igEpic = GetEpicWithoutExpiry(igEpic);
                lookupDictionary[igEpic] = ticker;

                File.WriteAllText(path, JsonConvert.SerializeObject(lookupDictionary));
            }
        }

        public static Dictionary<string, string> GetLookupByTicker()
        {
            Dictionary<string, string> reverseLookup;
            if (File.Exists(handMapped))
            {
                string fileContents = File.ReadAllText(path);
                reverseLookup = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContents);
            }
            else
            {
                throw new Exception("Cannot find mapping file.");
            }
            return reverseLookup;
        }

    }
}
