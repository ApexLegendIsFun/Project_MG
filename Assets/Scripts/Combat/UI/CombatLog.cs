using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace TurnBasedCombat.UI
{
    /// <summary>
    /// Displays combat messages and event log
    /// </summary>
    public class CombatLog : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private ScrollRect scrollRect;
        
        [Header("Settings")]
        [SerializeField] private int maxMessages = 50;
        [SerializeField] private bool autoScroll = true;
        [SerializeField] private Color playerColor = new Color(0.3f, 0.7f, 1f);
        [SerializeField] private Color enemyColor = new Color(1f, 0.3f, 0.3f);
        [SerializeField] private Color systemColor = Color.white;
        [SerializeField] private Color damageColor = new Color(1f, 0.5f, 0f);
        [SerializeField] private Color healColor = new Color(0.3f, 1f, 0.3f);

        private Queue<string> messages = new Queue<string>();

        private void Awake()
        {
            if (logText == null)
            {
                logText = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (scrollRect == null)
            {
                scrollRect = GetComponentInChildren<ScrollRect>();
            }

            Clear();
        }

        /// <summary>
        /// Add a system message
        /// </summary>
        public void LogMessage(string message)
        {
            AddMessage(message, systemColor);
        }

        /// <summary>
        /// Add a player action message
        /// </summary>
        public void LogPlayerAction(string characterName, string action)
        {
            string message = $"<b>{characterName}</b>이(가) {action}!";
            AddMessage(message, playerColor);
        }

        /// <summary>
        /// Add an enemy action message
        /// </summary>
        public void LogEnemyAction(string characterName, string action)
        {
            string message = $"<b>{characterName}</b>이(가) {action}!";
            AddMessage(message, enemyColor);
        }

        /// <summary>
        /// Add a damage message
        /// </summary>
        public void LogDamage(string targetName, int damage)
        {
            string message = $"<b>{targetName}</b>이(가) <b>{damage}</b> 데미지를 받았습니다!";
            AddMessage(message, damageColor);
        }

        /// <summary>
        /// Add a healing message
        /// </summary>
        public void LogHeal(string targetName, int amount)
        {
            string message = $"<b>{targetName}</b>이(가) <b>{amount}</b> HP를 회복했습니다!";
            AddMessage(message, healColor);
        }

        /// <summary>
        /// Add a status effect message
        /// </summary>
        public void LogStatusEffect(string targetName, string effectName)
        {
            string message = $"<b>{targetName}</b>에게 <b>{effectName}</b> 효과가 적용되었습니다!";
            AddMessage(message, systemColor);
        }

        /// <summary>
        /// Add a turn start message
        /// </summary>
        public void LogTurnStart(string characterName, bool isPlayer)
        {
            string message = $"--- <b>{characterName}</b>의 턴 ---";
            AddMessage(message, isPlayer ? playerColor : enemyColor);
        }

        /// <summary>
        /// Add a battle result message
        /// </summary>
        public void LogBattleResult(bool victory)
        {
            string message = victory ? "=== 승리! ===" : "=== 패배... ===";
            AddMessage(message, victory ? healColor : damageColor);
        }

        /// <summary>
        /// Add a colored message to the log
        /// </summary>
        private void AddMessage(string message, Color color)
        {
            // Convert color to hex
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            string coloredMessage = $"<color=#{hexColor}>{message}</color>";

            messages.Enqueue(coloredMessage);

            // Remove old messages if exceeding max
            while (messages.Count > maxMessages)
            {
                messages.Dequeue();
            }

            // Update display
            UpdateDisplay();
        }

        /// <summary>
        /// Update the log text display
        /// </summary>
        private void UpdateDisplay()
        {
            if (logText == null)
                return;

            logText.text = string.Join("\n", messages);

            // Auto scroll to bottom
            if (autoScroll && scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        /// <summary>
        /// Clear all messages
        /// </summary>
        public void Clear()
        {
            messages.Clear();
            if (logText != null)
            {
                logText.text = "전투 준비...";
            }
        }

        /// <summary>
        /// Add a custom colored message
        /// </summary>
        public void LogCustom(string message, Color color)
        {
            AddMessage(message, color);
        }
    }
}
