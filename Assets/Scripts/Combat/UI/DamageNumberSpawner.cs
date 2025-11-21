using UnityEngine;

namespace TurnBasedCombat.UI
{
    /// <summary>
    /// Spawns damage numbers at character positions
    /// </summary>
    public class DamageNumberSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject damageNumberPrefab;

        [Header("Settings")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1, 0);

        private void Awake()
        {
            if (canvas == null)
                canvas = FindObjectOfType<Canvas>();
        }

        /// <summary>
        /// Spawn a damage number at a world position
        /// </summary>
        public void SpawnDamage(Vector3 worldPosition, int amount, bool isCritical = false)
        {
            if (damageNumberPrefab == null || canvas == null)
                return;

            Vector3 spawnPos = worldPosition + spawnOffset;
            GameObject numberObj = Instantiate(damageNumberPrefab, canvas.transform);

            // Convert world position to screen position
            numberObj.transform.position = Camera.main.WorldToScreenPoint(spawnPos);

            DamageNumber damageNumber = numberObj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.ShowDamage(amount, isCritical);
            }
        }

        /// <summary>
        /// Spawn a heal number at a world position
        /// </summary>
        public void SpawnHeal(Vector3 worldPosition, int amount)
        {
            if (damageNumberPrefab == null || canvas == null)
                return;

            Vector3 spawnPos = worldPosition + spawnOffset;
            GameObject numberObj = Instantiate(damageNumberPrefab, canvas.transform);

            numberObj.transform.position = Camera.main.WorldToScreenPoint(spawnPos);

            DamageNumber damageNumber = numberObj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.ShowHeal(amount);
            }
        }

        /// <summary>
        /// Spawn custom text at a world position
        /// </summary>
        public void SpawnText(Vector3 worldPosition, string text, Color color)
        {
            if (damageNumberPrefab == null || canvas == null)
                return;

            Vector3 spawnPos = worldPosition + spawnOffset;
            GameObject numberObj = Instantiate(damageNumberPrefab, canvas.transform);

            numberObj.transform.position = Camera.main.WorldToScreenPoint(spawnPos);

            DamageNumber damageNumber = numberObj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.ShowText(text, color);
            }
        }
    }
}
