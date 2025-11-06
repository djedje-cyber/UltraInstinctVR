#region

using System;
using System.Collections;

using UnityEngine;

#endregion

namespace Xareus.Unity
{
    /// <summary>
    /// Screen fade effect by using a custom shader modifying the camera(s) rendered image
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class ScreenFadeEffect : MonoBehaviour
    {
        #region Statics

        /// <summary>
        /// The fade property of the material
        /// </summary>
        private static readonly int FADE_PROPERTY = Shader.PropertyToID("_FadeAmount");

        #endregion

        #region Fields

        /// <summary>
        /// Current fade value
        /// </summary>
        protected float fadeValue;

        /// <summary>
        /// The fade material
        /// </summary>
        protected Material mat;

        #endregion

        #region Unity

        /// <summary>
        /// This will use the camera rendering (source) and apply the fade effect
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (fadeValue > 0 && mat != null)
            {
                mat.SetFloat(FADE_PROPERTY, fadeValue);
                Graphics.Blit(source, destination, mat);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Instant Fade using black color
        /// </summary>
        /// <param name="cameras">Cameras on which the fade effect must be applied</param>
        /// <param name="fade">Fade value (1 : solid color, 0 : camera render)</param>
        public static void SetFade(Camera[] cameras, float fade)
        {
            SetFade(cameras, fade, Color.black);
        }

        /// <summary>
        /// Instant Fade using the specified color
        /// </summary>
        /// <param name="cameras">Cameras on which the fade effect must be applied</param>
        /// <param name="fade">Fade value (1 : solid color, 0 : camera render)</param>
        /// /// <param name="color">Color to use for the fade effect</param>
        public static void SetFade(Camera[] cameras, float fade, Color color)
        {
            if (cameras == null)
                throw new ArgumentNullException(nameof(cameras));

            foreach (Camera camera in cameras)
            {
                ScreenFadeEffect instance = GetScreenFade(camera);
                // Stop all coroutines to avoid previous fade commands for interfering
                instance.StopAllCoroutines();
                instance.mat.color = color;
                instance.fadeValue = fade;
            }
        }

        /// <summary>
        /// Fade using black color
        /// </summary>
        /// <param name="cameras">Cameras on which the fade effect must be applied</param>
        /// <param name="fade">Fade value (1 : solid color, 0 : camera render)</param>
        /// <param name="fadeDuration">Fade duration in seconds</param>
        public static void SmoothFade(Camera[] cameras, float fade, float fadeDuration)
        {
            SmoothFade(cameras, fade, fadeDuration, Color.black);
        }

        /// <summary>
        /// Fade using the specified color
        /// </summary>
        /// <param name="cameras">Cameras on which the fade effect must be applied</param>
        /// <param name="fade">Fade value (1 : solid color, 0 : camera render)</param>
        /// <param name="fadeDuration">Fade duration in seconds</param>
        /// <param name="color">Color to use for the fade effect</param>
        public static void SmoothFade(Camera[] cameras, float fade, float fadeDuration, Color color)
        {
            if (cameras == null)
                throw new ArgumentNullException(nameof(cameras));

            foreach (Camera camera in cameras)
            {
                ScreenFadeEffect instance = GetScreenFade(camera);
                // Stop all coroutines to avoid previous fade commands for interfering
                instance.StopAllCoroutines();
                instance.StartCoroutine(instance.FadeRoutine(fade, fadeDuration, color));
            }
        }

        /// <summary>
        /// Get the effect component (add the necessary elements if needed)
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected static ScreenFadeEffect GetScreenFade(Camera camera)
        {
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));

            ScreenFadeEffect res = camera.GetComponent<ScreenFadeEffect>();
            if (res == null) // Add the component if not present
                res = camera.gameObject.AddComponent<ScreenFadeEffect>();

            if (res.mat == null) // Set the material if not specified
            {
                res.mat = new Material(Resources.Load<Shader>("ScreenFade"));
                res.fadeValue = 0;
            }

            return res;
        }

        /// <summary>
        /// Fade routine
        /// </summary>
        /// <param name="targetFade"></param>
        /// <param name="fadeDuration"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private IEnumerator FadeRoutine(float targetFade, float fadeDuration, Color color)
        {
            mat.color = color;
            float initialFade = mat.GetFloat(FADE_PROPERTY);
            float completion = 0f;

            while (completion < 1f)
            {
                fadeValue = Mathf.Lerp(initialFade, targetFade, completion);
                yield return null;
                completion += UnityEngine.Time.deltaTime / fadeDuration;
            }

            // Ensure that the final fade value is correct
            fadeValue = targetFade;
        }

        #endregion
    }
}
