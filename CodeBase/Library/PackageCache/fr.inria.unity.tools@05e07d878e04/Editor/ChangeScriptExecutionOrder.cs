using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace InriaTools
{
    [InitializeOnLoad]
    public static class ChangeScriptExecutionOrder
    {
        #region Statics

        private static readonly string BATCH_MODE_PARAM = "-batchmode";
        private static readonly string SET_EXECUTION_ORDER_VALUE_COMMAND = "-scriptorder";

        #endregion

        #region Constructors

        static ChangeScriptExecutionOrder()
        {
            List<string> args = Environment.GetCommandLineArgs().ToList();

            if (args.Any(arg => arg.ToLower().Equals(BATCH_MODE_PARAM)))
            {
                Debug.LogFormat("ChangeScriptExecutionOrder will try to parse the command line to change the script execution order\n" +
                                "\t Use {0} \"namespaces.classname\" \"value\" for every script or component class for which you wish to change the execution order"
                    , SET_EXECUTION_ORDER_VALUE_COMMAND);
            }

            if (args.Any(arg => arg.ToLower().Equals(SET_EXECUTION_ORDER_VALUE_COMMAND))) // is an execution order change requested ?
            {
                Dictionary<string, int> classNameToNewExecutionOrderMap = new Dictionary<string, int>();
                int lastIndex = 0;
                while (lastIndex != -1)
                {
                    lastIndex = args.FindIndex(lastIndex, arg => arg.ToLower().Equals(SET_EXECUTION_ORDER_VALUE_COMMAND));
                    if (lastIndex >= 0 && lastIndex + 2 < args.Count)
                    {
                        string scriptToOrder = args[lastIndex + 1];
                        int order = int.Parse(args[lastIndex + 2]);
                        lastIndex++;
                        classNameToNewExecutionOrderMap.Add(scriptToOrder, order);
                    }
                }

                SetExecutionOrders(classNameToNewExecutionOrderMap);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the execution order of the given classes (use full name)
        /// </summary>
        /// <param name="newExecutionOrder"></param>
        public static void SetExecutionOrders(Dictionary<string, int> newExecutionOrder, bool reserializeWhenDone = true)
        {
            Debug.LogFormat("Will set the following script:orders : \n{0}", string.Join("\n", newExecutionOrder.Select(entry => entry.Key + ":" + entry.Value)));
            bool changedOne = false;
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                // if null, we may be dealing with an interface
                if (monoScript.GetClass() != null)
                {
                    if (newExecutionOrder.ContainsKey(monoScript.GetClass().FullName))
                    {
                        int order = newExecutionOrder[monoScript.GetClass().FullName];
                        // Setting the execution order might trigger a recompilation which will relaunch this script so check if the value is already correct before doing anything
                        if (MonoImporter.GetExecutionOrder(monoScript) != order)
                        {
                            Debug.LogFormat("Setting script {0} order to {1}", monoScript.GetClass().FullName, order);
                            MonoImporter.SetExecutionOrder(monoScript, order);
                            changedOne = true;
                        }
                        else
                        {
                            Debug.LogFormat("Script {0} order is already {1}", monoScript.GetClass().FullName, order);
                        }
                    }
                }
            }

            if (changedOne && reserializeWhenDone)
                AssetDatabase.ForceReserializeAssets();
        }

        #endregion
    }
}
