using System.Collections.Generic;

using UnityEngine.SceneManagement;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.SceneLoadedSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.SceneLoadedSensor", "Xareus.Unity.Librairies")]
    public class SceneLoadedSensor : AUnitySensor
    {
        #region Fields

        [EventContextEntry()]
        public static readonly string SCENEID = "sceneID";

        #endregion



        #region Fields

        [ConfigurationParameter("SceneID", Necessity.Optional, Description = "The Id of the scene to check. -1 (default) for any scene", InitialValue = -1)]
        protected int sceneID;

        protected bool sceneLoaded;

        /// <summary>
        /// The result
        /// </summary>
        protected SimpleDictionary eventContext = new();

        #endregion

        #region Constructors

        protected SceneLoadedSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeStopCheck()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        public override void SafeReset()
        {
            sceneLoaded = false;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        // Update is called once per frame
        public override Result SafeSensorCheck()
        {
            return new Result(sceneLoaded, eventContext);
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (sceneID < 0 || scene.buildIndex == sceneID)
            {
                sceneLoaded = true;
                SceneManager.sceneLoaded -= SceneLoaded;
                eventContext.SetValue(SCENEID, scene.buildIndex);
                UpdateScenario();
            }
        }

        #endregion
    }
}
