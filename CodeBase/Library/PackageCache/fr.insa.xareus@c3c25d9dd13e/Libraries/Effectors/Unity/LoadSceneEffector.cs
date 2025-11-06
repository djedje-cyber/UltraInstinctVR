using System.Collections.Generic;

using UnityEngine.SceneManagement;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.LoadSceneEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.LoadSceneEffector", "Xareus.Unity.Librairies")]
    public class LoadSceneEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("SceneID", Necessity.Required)]
        protected int sceneID;

        [ConfigurationParameter("Load Option", Necessity.Required)]
        protected LoadSceneMode loadMode;

        [ConfigurationParameter("Use Sync Unload", Necessity.Required)]
        protected bool sync;

        #endregion

        #region Constructors

        public LoadSceneEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            if (sync)
                SceneManager.LoadScene(sceneID, mode: loadMode);
            else
                SceneManager.LoadSceneAsync(sceneID, mode: loadMode);
        }

        #endregion
    }
}
