﻿using System;
using System.IO;
using System.Linq;

public static class SerializationUtils
{
    public static void SaveBrains(Brain[] brains, int generation, int lifespanMillis, string path)
    {
        var orderedBrains = brains.OrderByDescending(x => x.Score).ToArray();
        path = $"{path}/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-Brains/";
        foreach (var brain in orderedBrains)
            brain.Export(generation, path);
    }
        
    public static Brain[] LoadBrains(int count, string path)
    {
        var brains = new Brain[count]; 
        var loadedBrains = 0;

        while (loadedBrains != count)
        {
            foreach (var file in Directory.GetFiles(path, "*brain.txt").Concat(Directory.GetFiles(path, "*.json")))
            {
                brains[loadedBrains] = Brain.Import(file);
                loadedBrains++;
                if (loadedBrains == brains.Length)
                    break;
            }
        }

        return brains;
    }
}