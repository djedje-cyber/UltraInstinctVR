using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.ScreenFadeEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.ScreenFadeEffector", "Xareus.Unity.Librairies")]
    public class ScreenFadeEffector : AUnityEffector
    {
        #region Enums

        /// <summary>
        /// Fade type : Fade in to color or Fade out from color
        /// </summary>
        public enum FadeType
        {
            FADEIN,
            FADEOUT
        }

        #endregion

        #region Fields

        [ConfigurationParameter("Cameras", "All if not/none specified")]
        protected List<Camera> camerasToFade;

        [ConfigurationParameter("Color", "The solid color the fade effect will use. Default is black")]
        protected Color fadeColor = Color.black;

        [ConfigurationParameter("Duration", "Fade duration in seconds", Necessity.Required)]
        protected float fadeDuration;

        [ConfigurationParameter("Fade Type", "Fade in to color or Fade out from color", Necessity.Required)]
        protected FadeType fadeType;

        #endregion

        #region Constructors

        public ScreenFadeEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            Camera[] camerasToFadeArray = camerasToFade == null || camerasToFade.Count == 0
                ? Camera.allCameras
                : camerasToFade.ToArray();
            ScreenFadeEffect.SmoothFade(camerasToFadeArray, fadeType == FadeType.FADEIN ? 1 : 0, fadeDuration,
                fadeColor);
        }

        #endregion
    }
}
