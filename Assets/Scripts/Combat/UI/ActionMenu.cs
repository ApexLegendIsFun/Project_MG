using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace TurnBasedCombat.UI
{
    /// <summary>
    /// Displays available actions for the player
    /// </summary>
    public class ActionMenu : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button attackButton;
        [SerializeField] private Button skillsButton;
        [SerializeField] private Button itemsButton;
        [SerializeField] private Button defendButton;
        [SerializeField] private Button fleeButton;

        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject skillsPanel;
        [SerializeField] private GameObject itemsPanel;
        [SerializeField] private GameObject targetSelectionPanel;

        [Header("Skill List")]
        [SerializeField] private Transform skillListContainer;
        [SerializeField] private GameObject skillButtonPrefab;

        private TurnBasedCombat.Core.TurnManager turnManager;
        private TurnBasedCombat.Core.CombatCharacter currentCharacter;
        private CombatLog combatLog;
        private List<Button> skillButtons = new List<Button>();

        private void Awake()
        {
            // Setup button listeners
            if (attackButton != null)
                attackButton.onClick.AddListener(OnAttackClicked);

            if (skillsButton != null)
                skillsButton.onClick.AddListener(OnSkillsClicked);

            if (itemsButton != null)
                itemsButton.onClick.AddListener(OnItemsClicked);

            if (defendButton != null)
                defendButton.onClick.AddListener(OnDefendClicked);

            if (fleeButton != null)
                fleeButton.onClick.AddListener(OnFleeClicked);

            HideAllPanels();
        }

        /// <summary>
        /// Set the turn manager reference
        /// </summary>
        public void SetTurnManager(TurnBasedCombat.Core.TurnManager manager)
        {
            turnManager = manager;
        }

        /// <summary>
        /// Set the combat log reference
        /// </summary>
        public void SetCombatLog(CombatLog log)
        {
            combatLog = log;
        }

        /// <summary>
        /// Show the action menu for a character
        /// </summary>
        public void ShowMenu(TurnBasedCombat.Core.CombatCharacter character)
        {
            currentCharacter = character;
            ShowMainMenu();
        }

        /// <summary>
        /// Hide the action menu
        /// </summary>
        public void HideMenu()
        {
            HideAllPanels();
        }

        /// <summary>
        /// Show main action menu
        /// </summary>
        private void ShowMainMenu()
        {
            HideAllPanels();

            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
        }

        /// <summary>
        /// Hide all panels
        /// </summary>
        private void HideAllPanels()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);

            if (skillsPanel != null)
                skillsPanel.SetActive(false);

            if (itemsPanel != null)
                itemsPanel.SetActive(false);

            if (targetSelectionPanel != null)
                targetSelectionPanel.SetActive(false);
        }

        #region Button Handlers

        private void OnAttackClicked()
        {
            Debug.Log("Attack selected");
            
            if (combatLog != null && currentCharacter != null)
            {
                combatLog.LogPlayerAction(currentCharacter.CharacterName, "공격을 선택했습니다");
            }
            
            ShowTargetSelection(PerformAttack);
        }

        private void OnSkillsClicked()
        {
            Debug.Log("Skills selected");
            ShowSkillsMenu();
        }

        private void OnItemsClicked()
        {
            Debug.Log("Items selected");
            // TODO: Implement items menu
            ShowMainMenu();
        }

        private void OnDefendClicked()
        {
            Debug.Log("Defend selected");

            if (combatLog != null && currentCharacter != null)
            {
                combatLog.LogPlayerAction(currentCharacter.CharacterName, "방어 태세를 취했습니다");
            }

            if (turnManager != null && currentCharacter != null)
            {
                turnManager.ExecutePlayerAction(currentCharacter, (target) =>
                {
                    turnManager.StartCoroutine(currentCharacter.Defend());
                });
            }

            HideMenu();
        }

        private void OnFleeClicked()
        {
            Debug.Log("Flee selected");
            // TODO: Implement flee logic
            HideMenu();
        }

        #endregion

        #region Skills Menu

        /// <summary>
        /// Show skills menu with available abilities
        /// </summary>
        private void ShowSkillsMenu()
        {
            HideAllPanels();

            if (skillsPanel != null)
                skillsPanel.SetActive(true);

            // TODO: Populate with actual skills
            // For now, just show a back button
        }

        /// <summary>
        /// Create skill buttons
        /// </summary>
        private void PopulateSkills(List<TurnBasedCombat.Core.CombatAbility> skills)
        {
            // Clear existing buttons
            foreach (var button in skillButtons)
            {
                if (button != null)
                    Destroy(button.gameObject);
            }
            skillButtons.Clear();

            // Create new buttons
            foreach (var skill in skills)
            {
                if (skillButtonPrefab != null && skillListContainer != null)
                {
                    var buttonObj = Instantiate(skillButtonPrefab, skillListContainer);
                    var button = buttonObj.GetComponent<Button>();

                    if (button != null)
                    {
                        var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                        if (text != null)
                            text.text = $"{skill.AbilityName} (MP: {skill.MPCost})";

                        button.onClick.AddListener(() => OnSkillSelected(skill));
                        skillButtons.Add(button);
                    }
                }
            }
        }

        /// <summary>
        /// Called when a skill is selected
        /// </summary>
        private void OnSkillSelected(TurnBasedCombat.Core.CombatAbility skill)
        {
            Debug.Log($"Skill selected: {skill.AbilityName}");

            if (!skill.CanUse(currentCharacter))
            {
                Debug.Log("Cannot use this skill!");
                return;
            }

            ShowTargetSelection((target) => PerformSkill(skill, target));
        }

        #endregion

        #region Target Selection

        /// <summary>
        /// Show target selection UI
        /// </summary>
        private void ShowTargetSelection(System.Action<TurnBasedCombat.Core.CombatCharacter> onTargetSelected)
        {
            HideAllPanels();

            if (targetSelectionPanel != null)
                targetSelectionPanel.SetActive(true);

            // TODO: Create target selection buttons
            // For now, automatically select first enemy
            if (turnManager != null && turnManager.EnemyCharacters.Count > 0)
            {
                var firstAliveEnemy = turnManager.EnemyCharacters.Find(e => e.IsAlive);
                if (firstAliveEnemy != null)
                {
                    onTargetSelected?.Invoke(firstAliveEnemy);
                    HideMenu();
                }
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Perform basic attack
        /// </summary>
        private void PerformAttack(TurnBasedCombat.Core.CombatCharacter target)
        {
            if (turnManager != null && currentCharacter != null)
            {
                turnManager.ExecutePlayerAction(target, (t) =>
                {
                    turnManager.StartCoroutine(currentCharacter.Attack(t));
                });
            }
        }

        /// <summary>
        /// Perform skill
        /// </summary>
        private void PerformSkill(TurnBasedCombat.Core.CombatAbility skill, TurnBasedCombat.Core.CombatCharacter target)
        {
            if (turnManager != null && currentCharacter != null)
            {
                turnManager.ExecutePlayerAction(target, (t) =>
                {
                    turnManager.StartCoroutine(skill.Execute(currentCharacter, t));
                });
            }
        }

        #endregion
    }
}
