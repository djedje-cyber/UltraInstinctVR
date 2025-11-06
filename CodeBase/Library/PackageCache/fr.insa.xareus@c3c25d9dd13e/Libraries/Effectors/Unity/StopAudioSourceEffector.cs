using InriaTools;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("FIVE.Utils.Effectors.StopAudioSourceEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.Unity.StopAudioSourceEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.StopAudioSourceEffector", "Xareus.Unity.Librairies")]
    public class StopAudioSourceEffector : AUnityEffector
    {
        #region Fields

#pragma warning disable 0649

        [ConfigurationParameter("Audio Source", Necessity.Required)]
        protected AudioSource audioSource;

        [ConfigurationParameter("Finish Current Clip",
            "Finish the current clip, meaning disabling the loop if it's on.\n" +
            "Note this does not prevent from enabling back the loop later and canceling the stop",
            Necessity.Required)]
        protected bool finishClip;

        [ConfigurationParameter("Wait For End",
            "Wait for the current clip to be stopped (instantly or at the end of a loop depending on other parameters) before passing the transition",
            Necessity.Required)]
        protected bool waitForEnd;

#pragma warning restore 0649

        protected bool finished = true;

        /// <summary>
        /// Which audio clip is being played ?
        /// </summary>
        protected AudioClip audioClip;

        /// <summary>
        /// Time of the current audio clip
        /// </summary>
        private float currentClipTime;

        #endregion

        #region Properties

        /// <summary>
        /// Override to have a non-ponctual effector
        /// </summary>
        public override bool Finished => finished;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="event"></param>
        /// <param name="parameters"></param>
        /// <param name="externalContext"></param>
        /// <param name="scenarioContext"></param>
        /// <param name="sequenceContext"></param>
        /// <param name="eventContext"></param>
        public StopAudioSourceEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            // get the current clip and its timing
            audioClip = audioSource.clip;
            currentClipTime = audioSource.time;

            // finish now
            if (!finishClip)
            {
                audioSource.Stop();
            }
            else
            {
                // disable loop so it stops by itself. Note this deosn't prevent anyone from enabling the loop later and cancelling the stop
                audioSource.loop = false;

                // If non-ponctual
                if (waitForEnd)
                {
                    finished = false;
                    UnityThreadExecute.Instance.StartCoroutine(CheckAudioState());
                }
            }
        }

        /// <summary>
        /// Check every frame if the audiosource is still playing or if the audio clip has changed
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CheckAudioState()
        {
            while (audioSource.isPlaying && audioSource.clip == audioClip && audioSource.time >= currentClipTime)
            {
                currentClipTime = audioSource.time;
                yield return null;
            }
            finished = true;
        }

        #endregion
    }
}
