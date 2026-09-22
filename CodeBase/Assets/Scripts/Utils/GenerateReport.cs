using System;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;



namespace GenerateReportSpace
{

    public class GenerateReport : MonoBehaviour
    {
        void Start()
        {
            Export();
        }

        public static void CountTestGeneratedLogs(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("The folder games logs does not exists !");
                return;
            }

            int count = 0;
            string[] lines = File.ReadAllLines(filePath);

            count = lines.Count(line => line.Contains("TestGenerated"));


            Console.WriteLine($"Number of log containing 'TestGenerated' : {count}");
        }


        public static void Export()
        {
            string xmlPath = "CodeCoverage/Report/Summary.xml";


            string folderPath = Path.Combine(Application.dataPath, "..", "Logs", "coverage");
            Debug.Log("Folder path: " + folderPath);
            string filename = GenerateFileName();
            string csvPath = Path.Combine(folderPath,filename);


            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log("Created folder: " + folderPath);
            }

            if (!File.Exists(xmlPath))
            {
                Debug.LogError("Coverage Summary not found at: " + xmlPath);
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            XmlNode summary = doc.SelectSingleNode("//CoverageReport/Summary");

            if (summary == null)
            {
                Debug.LogError("Summary node not found in coverage report.");
                return;
            }

            string totalMethods = summary["Totalmethods"].InnerText;
            string coveredMethods = summary["Coveredmethods"].InnerText;
            string methodCoverage = summary["Methodcoverage"].InnerText;
            string lineCoverage = summary["Linecoverage"].InnerText;
            string interactableCoverage = InteractableCoverage().ToString();
            using (StreamWriter writer = new StreamWriter(csvPath))
            {
                writer.WriteLine("MethodCoverage,TotalMethods,CoveredMethods,LineCoverage,InteractableCoverage");
                writer.WriteLine($"{methodCoverage},{totalMethods},{coveredMethods},{lineCoverage},{interactableCoverage}");
            }

            Debug.Log("Coverage summary CSV generated at: " + csvPath);
        }



        public static float InteractableCoverage()
        {

            //Get each interactable object in the scene, even the inactive ones
            XRBaseInteractable[] interactables = UnityEngine.Object.FindObjectsByType<XRBaseInteractable>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            float total = interactables.Length;
            float objectFound = CountLinesInFoundObjects();
            float coverage = (objectFound / total) * 100;

            Debug.Log($"Total Interactable objects in scene: {coverage} from InteractableCoverage");

            //Get the percentage of interactable object found
            return coverage;
        }


        public static int CountLinesInFoundObjects()
        {

            string folderPath = Path.Combine(Application.dataPath, "..", "Logs");
            
            string path = Path.Combine(folderPath, "FoundObject.txt");

            if (!File.Exists(path))
            {
                Debug.LogError($" File not found: {path}");
                return 0;
            }

            try
            {
                int lineCount = File.ReadAllLines(path).Length;
                Debug.Log($"FoundObjects.txt contains {lineCount} lines.");
                return lineCount;
            }
            catch (IOException e)
            {
                Debug.LogError($" Error reading file: {e.Message}");
                return 0;
            }
        }


        private static string GenerateFileName()
        {
            string uuid = Guid.NewGuid().ToString().Substring(0, 5);
            string date = DateTime.Now.ToString("ddMMyyyy");
            string resultDirectory = Path.Combine(Application.dataPath, "..", "Logs", "coverage");
            int runNumber = 1;
            
            if (!Directory.Exists(resultDirectory))
            {
                Directory.CreateDirectory(resultDirectory);
            }

            runNumber = Directory.GetFiles(resultDirectory, "coverage_*.csv").Length + 1;
           

            return $"coverage_{uuid}_{date}_Run_{runNumber}.csv";
        }


    }



}