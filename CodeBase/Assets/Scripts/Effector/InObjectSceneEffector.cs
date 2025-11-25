using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;



namespace InObjectSceneEffectorSpace {

    ///<summary>
    ///Class <c>InObjectSceneEffector</c> checks if the player teleports into covered objects in a Unity scene
    ///</summary>

    public class InObjectSceneEffector : AUnityEffector
    {
        [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
        protected float teleportDistanceThreshold = 0.0f;

        [ConfigurationParameter("GameObjectToObserve", Necessity.Required)]
        protected GameObject gameObjectToObserve;
        
        private Vector3 lastPosition;


        private readonly string filePath = "Logs/FoundObject.txt";

        private readonly List<Vector3> coveredObjectPositions = new List<Vector3>();  // List to store positions

        public InObjectSceneEffector(Xareus.Scenarios.Event @event,
            Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
            IContext externalContext,
            IContext scenarioContext,
            IContext sequenceContext,
            IContext anotherContext)
            : base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext, anotherContext))

        { }



        ///<summary>
        ///Method  <c>SafeReset</c> resets the effector state before execution.
        ///</summary>
        public override void SafeReset()
        {
            LoadCoveredObjectPositions();

            lastPosition = GetPlayerPosition();
        }



        ///<summary>
        ///Method <c>SafeEffectorUpdate</c> checks if the player has teleported into any covered objects.
        ///</summary>
        public override void SafeEffectorUpdate()
        {
            Vector3 currentPosition = GetPlayerPosition();

            // Tolerance in position
            float tolerance = 4f;
            Debug.Log("TestGenerated - InObjectSceneTeleportation");
            bool flag = true;
            foreach (Vector3 coveredPosition in coveredObjectPositions)
            {
                float distance = Vector3.Distance(currentPosition, coveredPosition);
                // If the distance between the current player position and the covered position is less than tolerance
                if (distance < tolerance)
                {
                    Debug.LogError("ORACLE InObjectScene - TestFailed - Teleportation into covered object detected: " + currentPosition);
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                Debug.Log("ORACLE InObjectScene - TestPassed - No teleportation into covered object detected: " + currentPosition);
            }
           
        }


        ///<summary>
        ///Method <c>LoadCoveredObjectPositions</c> loads covered object positions from a text file.
        ///</summary>
        private void LoadCoveredObjectPositions()
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("Position file does not exist at path: " + filePath);
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            string pattern = @"\(([^)]+)\)";

            foreach (string line in lines)
            {
                ParseAndAddPosition(line, pattern);
            }
        }

        ///<summary>
        ///Method <c>ParseAndAddPosition</c> parses a line to extract coordinates and adds them to the list.
        ///</summary>
        ///<param name="line"></param>
        ///<param name="pattern"></param>

        private void ParseAndAddPosition(string line, string pattern)
        {
            Match match = Regex.Match(line, pattern);
            if (!match.Success)
            {
                Debug.LogWarning("No coordinates found in parentheses on line: " + line);
                return;
            }

            string[] coordinates = match.Groups[1].Value.Split(',');
            if (coordinates.Length != 3)
            {
                Debug.LogWarning("Line does not contain 3 coordinates: " + line);
                return;
            }

            TryAddPosition(coordinates, line);
        }

        ///<summary>
        ///Method <c>TryAddPosition</c> attempts to parse coordinates and add them to the list.
        ///</summary>
        ///<param name="coordinates"></param>
        ///<param name="line"></param>

        private void TryAddPosition(string[] coordinates, string line)
        {
            try
            {
                float x = float.Parse(coordinates[0].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                float y = float.Parse(coordinates[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                float z = float.Parse(coordinates[2].Trim(), CultureInfo.InvariantCulture.NumberFormat);

                coveredObjectPositions.Add(new Vector3(x, y, z));
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading coordinates on line: " + line + "\n" + e.Message);
            }
        }

        ///<summary>
        ///Method <c>GetPlayerPosition</c> retrieves the current position of the player.
        ///</summary>
        ///<returns></returns>
        private Vector3 GetPlayerPosition()
        {
            return gameObjectToObserve.transform.position;
        }
    }

}