using System.IO;
using System;
using UnityEngine;

namespace VRLoggerSpace
{
    public class VRLogger
    {
        public void ClearFoundObjectsFile()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string logFilePath = Path.Combine(projectRoot, "Logs", "game_logs.txt");

            string dir = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //  version safe
            using (FileStream fs = new FileStream(
                logFilePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.ReadWrite))
            {
                // vide le fichier
            }

            Debug.Log("The game_logs.txt file has been cleared.");
        }
    }
}