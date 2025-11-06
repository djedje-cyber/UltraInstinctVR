using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("FIVE.Utils.Effectors.StartDisplayVideoClipEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.Unity.StartDisplayVideoClipEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.StartDisplayVideoClipEffector", "Xareus.Unity.Librairies")]
    public class StartDisplayVideoClipEffector : AUnityEffector
    {
        #region Fields

#pragma warning disable 0649

        //Source
        [ConfigurationParameter("videoClip", Necessity.Required)]
        private VideoClip videoClip;

        //Output
        [ConfigurationParameter("UI Raw Image", Necessity.Required)]
        private UnityEngine.UI.RawImage RawImage;

#pragma warning restore 0649

        //Media
        private VideoPlayer videoPlayer;

        private AudioSource audioSource;

        #endregion

        #region Constructors

        public StartDisplayVideoClipEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            GameObject go = new(videoClip.name);

            //Creates medium
            videoPlayer = go.AddComponent<VideoPlayer>();

            //Adds audio
            audioSource = go.AddComponent<AudioSource>();
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);

            //Additionnal configuration
            videoPlayer.isLooping = true;
            videoPlayer.playOnAwake = false;
            audioSource.playOnAwake = false;

            //Sets source
            if (videoClip != null)
            {
                videoPlayer.source = VideoSource.VideoClip;
                videoPlayer.clip = videoClip;
            }

            //Prime video player
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }

        private void OnVideoPrepared(object sender)
        {
            RawImage.texture = videoPlayer.texture;

            videoPlayer.Play();
            audioSource.Play();

            RawImage.enabled = true;
        }

        #endregion
    }
}
