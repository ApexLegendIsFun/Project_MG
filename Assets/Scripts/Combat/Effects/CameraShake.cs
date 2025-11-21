using System.Collections;
using UnityEngine;

namespace TurnBasedCombat.Effects
{
    /// <summary>
    /// Provides camera shake effects for combat feedback
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float defaultDuration = 0.2f;
        [SerializeField] private float defaultMagnitude = 0.1f;

        private Camera mainCamera;
        private Vector3 originalPosition;
        private bool isShaking = false;

        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                originalPosition = mainCamera.transform.localPosition;
            }
        }

        /// <summary>
        /// Trigger camera shake with default values
        /// </summary>
        public void Shake()
        {
            Shake(defaultDuration, defaultMagnitude);
        }

        /// <summary>
        /// Trigger camera shake with custom parameters
        /// </summary>
        public void Shake(float duration, float magnitude)
        {
            if (!isShaking)
            {
                StartCoroutine(ShakeCoroutine(duration, magnitude));
            }
        }

        /// <summary>
        /// Camera shake coroutine
        /// </summary>
        private IEnumerator ShakeCoroutine(float duration, float magnitude)
        {
            if (mainCamera == null)
                yield break;

            isShaking = true;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = originalPosition;
            isShaking = false;
        }

        /// <summary>
        /// Stop shaking immediately
        /// </summary>
        public void StopShake()
        {
            if (mainCamera != null)
            {
                StopAllCoroutines();
                mainCamera.transform.localPosition = originalPosition;
                isShaking = false;
            }
        }

        /// <summary>
        /// Shake for light hit
        /// </summary>
        public void ShakeLight()
        {
            Shake(0.1f, 0.05f);
        }

        /// <summary>
        /// Shake for medium hit
        /// </summary>
        public void ShakeMedium()
        {
            Shake(0.2f, 0.1f);
        }

        /// <summary>
        /// Shake for heavy hit
        /// </summary>
        public void ShakeHeavy()
        {
            Shake(0.3f, 0.2f);
        }
    }
}
