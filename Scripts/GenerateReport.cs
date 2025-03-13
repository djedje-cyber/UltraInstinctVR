using System;
using System.IO;

class GenerateReport
{
    static void Main()
    {
        string filePath = "Assets/Scripts/game_logs.txt";
        CountTestGeneratedLogs(filePath);
    }

    static void CountTestGeneratedLogs(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Le fichier game_logs.txt n'existe pas !");
            return;
        }

        int count = 0;
        string[] lines = File.ReadAllLines(filePath);
        
        foreach (string line in lines)
        {
            if (line.Contains("TestGenerated"))
            {
                count++;
            }
        }
        
        Console.WriteLine($"Nombre de logs contenant 'TestGenerated' : {count}");
    }
}