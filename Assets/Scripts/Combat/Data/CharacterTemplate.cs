using UnityEngine;
using System.Collections.Generic;

namespace TurnBasedCombat.Data
{
    /// <summary>
    /// Template for creating characters with predefined stats and abilities
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacterTemplate", menuName = "Combat/Character Template")]
    public class CharacterTemplate : ScriptableObject
    {
        [Header("Character Info")]
        [SerializeField] private string characterName = "Character";
        [SerializeField] private string description = "Character description";
        [SerializeField] private bool isPlayer = true;

        [Header("Stats")]
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int maxMP = 50;
        [SerializeField] private int attack = 15;
        [SerializeField] private int defense = 8;
        [SerializeField] private int speed = 12;

        [Header("Visuals")]
        [SerializeField] private Color spriteColor = Color.white;
        [SerializeField] private CharacterShape shape = CharacterShape.Square;
        [SerializeField] private Vector2 spriteSize = new Vector2(1f, 1f);
        [SerializeField] private Sprite customSprite;

        [Header("Abilities")]
        [SerializeField] private List<Core.CombatAbility> abilities = new List<Core.CombatAbility>();

        [Header("Starting Status Effects")]
        [SerializeField] private List<StatusEffectData> startingEffects = new List<StatusEffectData>();

        // Properties
        public string CharacterName => characterName;
        public string Description => description;
        public bool IsPlayer => isPlayer;
        public int MaxHP => maxHP;
        public int MaxMP => maxMP;
        public int Attack => attack;
        public int Defense => defense;
        public int Speed => speed;
        public Color SpriteColor => spriteColor;
        public CharacterShape Shape => shape;
        public Vector2 SpriteSize => spriteSize;
        public Sprite CustomSprite => customSprite;
        public List<Core.CombatAbility> Abilities => abilities;
        public List<StatusEffectData> StartingEffects => startingEffects;

        /// <summary>
        /// Create a combat character from this template
        /// </summary>
        public Core.CombatCharacter CreateCharacter(Vector3 position, Transform parent = null)
        {
            // Create character GameObject
            GameObject charObj = new GameObject(characterName);
            charObj.transform.position = position;

            if (parent != null)
                charObj.transform.SetParent(parent);

            // Add visual representation
            GameObject spriteObj = new GameObject("Sprite");
            spriteObj.transform.SetParent(charObj.transform);
            spriteObj.transform.localPosition = Vector3.zero;

            SpriteRenderer spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();

            // Set sprite
            if (customSprite != null)
            {
                spriteRenderer.sprite = customSprite;
            }
            else
            {
                spriteRenderer.sprite = CreateShapeSprite(shape);
            }

            spriteRenderer.color = spriteColor;
            spriteObj.transform.localScale = new Vector3(spriteSize.x, spriteSize.y, 1f);

            // Add combat position
            GameObject posObj = new GameObject("CombatPosition");
            posObj.transform.SetParent(charObj.transform);
            posObj.transform.localPosition = Vector3.zero;

            // Add CombatCharacter component
            Core.CombatCharacter character = charObj.AddComponent<Core.CombatCharacter>();
            character.InitializeCharacter(characterName, isPlayer, maxHP, maxMP, attack, defense, speed);
            character.SetSpriteRenderer(spriteRenderer);
            character.SetCombatPosition(posObj.transform);

            // Apply starting effects (would need to be done after battle starts)
            // This is stored for later application

            return character;
        }

        /// <summary>
        /// Create a sprite based on shape
        /// </summary>
        private Sprite CreateShapeSprite(CharacterShape shapeType)
        {
            Texture2D texture = new Texture2D(100, 100);
            Color[] pixels = new Color[100 * 100];

            switch (shapeType)
            {
                case CharacterShape.Square:
                    // Fill entire texture
                    for (int i = 0; i < pixels.Length; i++)
                        pixels[i] = Color.white;
                    break;

                case CharacterShape.Circle:
                    // Create circle
                    Vector2 center = new Vector2(50, 50);
                    float radius = 45f;
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            float dist = Vector2.Distance(new Vector2(x, y), center);
                            pixels[y * 100 + x] = dist <= radius ? Color.white : Color.clear;
                        }
                    }
                    break;

                case CharacterShape.Triangle:
                    // Create triangle
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            // Simple triangle: top vertex at (50,90), base from (10,10) to (90,10)
                            bool inTriangle = y < 90 && x > (10 + (y * 0.4f)) && x < (90 - (y * 0.4f));
                            pixels[y * 100 + x] = inTriangle ? Color.white : Color.clear;
                        }
                    }
                    break;

                case CharacterShape.Diamond:
                    // Create diamond
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            int distX = Mathf.Abs(x - 50);
                            int distY = Mathf.Abs(y - 50);
                            pixels[y * 100 + x] = (distX + distY) <= 45 ? Color.white : Color.clear;
                        }
                    }
                    break;
            }

            texture.SetPixels(pixels);
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f), 100f);
        }

        /// <summary>
        /// Get stats as a struct
        /// </summary>
        public CharacterStatsData GetStatsData()
        {
            return new CharacterStatsData
            {
                maxHP = this.maxHP,
                maxMP = this.maxMP,
                attack = this.attack,
                defense = this.defense,
                speed = this.speed
            };
        }
    }

    /// <summary>
    /// Shape options for auto-generated character sprites
    /// </summary>
    public enum CharacterShape
    {
        Square,
        Circle,
        Triangle,
        Diamond
    }

    /// <summary>
    /// Simple struct for character stats
    /// </summary>
    [System.Serializable]
    public struct CharacterStatsData
    {
        public int maxHP;
        public int maxMP;
        public int attack;
        public int defense;
        public int speed;
    }

    /// <summary>
    /// Data for a status effect to apply
    /// </summary>
    [System.Serializable]
    public class StatusEffectData
    {
        public string effectName = "Effect";
        public Core.StatusEffectType effectType = Core.StatusEffectType.Poison;
        public int duration = 3;
        public int power = 5;
    }
}
