using System;
using System.IO;
using System.Linq;


namespace GenerateReportSpace
{

    public class GenerateReport
    {
        public static void Main()
        {
            string filePath = "Assets/Scripts/game_logs.txt";
            CountTestGeneratedLogs(filePath);
        }

        public static void CountTestGeneratedLogs(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Le fichier game_logs.txt n'existe pas !");
                return;
            }

            int count = 0;
            string[] lines = File.ReadAllLines(filePath);

            count = lines.Count(line => line.Contains("TestGenerated"));


            Console.WriteLine($"Nombre de logs contenant 'TestGenerated' : {count}");
        }
    }
}