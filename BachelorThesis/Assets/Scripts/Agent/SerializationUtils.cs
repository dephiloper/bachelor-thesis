using System;
using System.IO;
using System.Linq;

namespace Agent
{
    public static class SerializationUtils
    {
        public static void SaveBrains(Brain[] brains, int generation, int lifespanMillis, string path)
        {
            var orderedBrains = brains.OrderByDescending(x => x.Score).ToArray();
            path = $"{path}/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-Brains/";
            foreach (var brain in orderedBrains)
                brain.Export(generation, lifespanMillis, path);
        }
        
        public static Brain[] LoadBrains(int count, string path)
        {
            var brains = new Brain[count]; 
            var loadedBrains = 0;

            while (loadedBrains != count)
            {
                foreach (var file in Directory.GetFiles(path, "*brain.json"))
                {
                    brains[loadedBrains] = Brain.Import(File.ReadAllText(file));
                    loadedBrains++;
                    if (loadedBrains == brains.Length)
                        break;
                }
            }

            return brains;
        }
    }
}