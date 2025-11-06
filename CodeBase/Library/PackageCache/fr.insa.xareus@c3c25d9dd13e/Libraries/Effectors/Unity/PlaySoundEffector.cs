using InriaTools;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.PlaySoundEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.PlaySoundEffector", "Xareus.Unity.Librairies")]
    public class PlaySoundEffector : AUnityEffector
    {
        #region Fields

#pragma warning disable 0649

        [ConfigurationParameter("Audio Source", Necessity.Required)]
        protected AudioSource audioSource;

        [ConfigurationParameter("Audio Clip", Necessity.Required)]
        protected AudioClip audioClip;

        [ConfigurationParameter("Volume", Necessity.Optional)]
        protected float volume = -1;

        [ConfigurationParameter("Loop", Necessity.Required)]
        protected bool loop;

        [ConfigurationParameter("Wait For End", "Wait for the clip to finish once before passing the transition", Necessity.Required)]
        protected bool waitForEnd;

#pragma warning restore 0649

        protected bool finished = true;

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
        public PlaySoundEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            // Set everything
            audioSource.clip = audioClip;
            audioSource.loop = loop;
            if (volume > 0)
                audioSource.volume = volume;
            audioSource.enabled = true;
            audioSource.Play();

            // If non-ponctual
            if (waitForEnd)
            {
                finished = false;
                UnityThreadExecute.Instance.StartCoroutine(CheckAudioState());
            }
        }

        /// <summary>
        /// Check every frame if the audiosource is still playing or if the audio clip has changed
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CheckAudioState()
        {
            float currentClipTime = 0;
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
