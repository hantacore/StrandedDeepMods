using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PrefabsImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            // read both files
            string json = File.ReadAllText("assets/unnamed asset-resources.assets-3081.dat");
            dynamic prefabs = JsonConvert.DeserializeObject(json);

            string[] IDs = File.ReadAllLines("assets/unnamed asset-resources.assets-3071.dat");
            Dictionary<string, string> prefabGuids = new Dictionary<string, string>();
            foreach(string line in IDs)
            {
                string[] tokens = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                prefabGuids.Add(tokens[0], tokens[1]);
            }

            foreach(dynamic prefab in prefabs)
            {
                string key = prefab.Name;
                string id = prefab.Value["Id"];

                if (prefabGuids.ContainsKey(id))
                {
                    Console.WriteLine("prefabs.Add(\"" + key.Substring(key.LastIndexOf(']') + 1) + "\", \"" + prefabGuids[id] + "\");//" + id + "\"");
                }
            }

            Console.ReadLine();
        }
    }
}
