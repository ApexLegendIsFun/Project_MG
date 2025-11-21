# Quick Reference Card

## 🎯 Essential Code Snippets

### Starting Combat

```csharp
// Get reference to turn manager
TurnManager turnManager = FindObjectOfType<TurnManager>();

// Add characters
turnManager.AddCharacter(playerCharacter, isPlayer: true);
turnManager.AddCharacter(enemyCharacter, isPlayer: false);

// Start battle
turnManager.StartBattle();
```

### Creating a Character

```csharp
// Add component to GameObject
var character = gameObject.AddComponent<CombatCharacter>();

// Stats are configured in Inspector or via reflection
```

### Executing Actions

```csharp
// Attack
StartCoroutine(character.Attack(target));

// Defend
StartCoroutine(character.Defend());

// Heal
character.Heal(amount: 30);

// Take Damage
int actualDamage = character.TakeDamage(damage: 50);
```

### Status Effects

```csharp
// Create effect
var poison = new StatusEffect(
    name: "Poison",
    type: StatusEffectType.Poison,
    duration: 3,
    power: 5
);

// Apply to character
character.AddStatusEffect(poison);

// Remove effect
character.RemoveStatusEffect(poison);

// Get active effects
List<StatusEffect> effects = character.GetActiveEffects();
```

### Creating Abilities

```csharp
// In Unity Editor:
// Right-click → Create → Combat → Ability

// Using ability in code:
StartCoroutine(ability.Execute(user, target));

// Check if can use
bool canUse = ability.CanUse(character);
```

### Events

```csharp
// Subscribe to combat events
turnManager.OnTurnStart.AddListener((character) => {
    Debug.Log($"{character.CharacterName}'s turn!");
});

turnManager.OnStateChanged.AddListener((state) => {
    Debug.Log($"State: {state}");
});

// Subscribe to character events
character.OnDamageTaken.AddListener((damage) => {
    Debug.Log($"Took {damage} damage!");
});

character.OnHealed.AddListener((amount) => {
    Debug.Log($"Healed {amount} HP!");
});
```

### Visual Effects

```csharp
// Camera shake
CameraShake shake = Camera.main.GetComponent<CameraShake>();
shake.ShakeLight();   // Small
shake.ShakeMedium();  // Medium
shake.ShakeHeavy();   // Large

// Damage numbers
DamageNumberSpawner spawner = FindObjectOfType<DamageNumberSpawner>();
spawner.SpawnDamage(position, amount: 50, isCritical: false);
spawner.SpawnHeal(position, amount: 30);
spawner.SpawnText(position, "MISS!", Color.gray);

// Combat effects
var effect = CombatEffect.Create(prefab, position, color);
effect.Play();
```

### Checking Combat State

```csharp
// Get current state
CombatState state = turnManager.CurrentState;

// Check specific states
if (state == CombatState.PlayerTurn)
{
    // Show action menu
}

// Get current character
CombatCharacter current = turnManager.CurrentCharacter;
```

### Stats Access

```csharp
CharacterStats stats = character.Stats;

// Read stats
int currentHP = stats.CurrentHP;
int maxHP = stats.MaxHP;
int attack = stats.Attack;
bool isAlive = stats.IsAlive;

// Modify stats
stats.TakeDamage(amount: 30);
stats.Heal(amount: 20);
stats.ConsumeMP(amount: 10);
stats.RestoreMP(amount: 15);
stats.ModifyStat(StatType.Attack, amount: 5, permanent: false);
```

### Player Input Handling

```csharp
// Execute player action
turnManager.ExecutePlayerAction(target, (t) => {
    StartCoroutine(currentCharacter.Attack(t));
});

// With ability
turnManager.ExecutePlayerAction(target, (t) => {
    StartCoroutine(ability.Execute(currentCharacter, t));
});
```

## 📊 Common Patterns

### Custom Character Class

```csharp
using TurnBasedCombat.Core;

public class Mage : CombatCharacter
{
    [SerializeField] private List<CombatAbility> spells;

    public override IEnumerator Attack(CombatCharacter target)
    {
        // Custom attack logic
        PlayAnimation("Cast");
        yield return new WaitForSeconds(0.5f);

        // Use base attack or custom
        yield return base.Attack(target);
    }
}
```

### Custom Enemy AI

```csharp
private IEnumerator SmartEnemyTurn(CombatCharacter enemy)
{
    yield return new WaitForSeconds(1f);

    // Choose lowest HP player
    var target = playerCharacters
        .Where(p => p.IsAlive)
        .OrderBy(p => p.Stats.CurrentHP)
        .FirstOrDefault();

    if (target != null)
    {
        // Use ability if enough MP
        if (enemy.Stats.CurrentMP >= 15)
        {
            yield return StartCoroutine(specialAbility.Execute(enemy, target));
        }
        else
        {
            yield return StartCoroutine(enemy.Attack(target));
        }
    }
}
```

### Victory/Defeat Handling

```csharp
void OnEnable()
{
    turnManager.OnBattleEnd.AddListener(HandleBattleEnd);
}

void HandleBattleEnd()
{
    if (turnManager.CurrentState == CombatState.Victory)
    {
        // Show victory screen
        // Give rewards
        // Play victory music
    }
    else if (turnManager.CurrentState == CombatState.Defeat)
    {
        // Show defeat screen
        // Offer retry/game over options
    }
}
```

### Dynamic HUD Updates

```csharp
void SetupCharacter(CombatCharacter character, CombatHUD hud)
{
    hud.SetCharacter(character);

    // Custom updates
    character.Stats.OnHPChanged += (current, max) => {
        float percent = (float)current / max;
        if (percent <= 0.25f)
        {
            // Low health warning
            PlayLowHealthWarning();
        }
    };
}
```

### Combo System

```csharp
private int comboCount = 0;
private float lastAttackTime = 0;
private float comboWindow = 2f;

void OnAttack(CombatCharacter attacker)
{
    if (Time.time - lastAttackTime <= comboWindow)
    {
        comboCount++;

        if (comboCount >= 3)
        {
            // Bonus damage or effect
            SpawnText("COMBO!", Color.yellow);
        }
    }
    else
    {
        comboCount = 1;
    }

    lastAttackTime = Time.time;
}
```

## 🎨 UI Customization

### Custom Damage Number Colors

```csharp
public class CustomDamageNumber : DamageNumber
{
    public void ShowCritical(int amount)
    {
        ShowDamage(amount, isCritical: true);
        // Add particle burst
        // Play sound
        // Scale up animation
    }

    public void ShowMiss()
    {
        ShowText("MISS", Color.gray);
    }

    public void ShowBlock(int blocked)
    {
        ShowText($"Block {blocked}", Color.blue);
    }
}
```

### Turn Order Display

```csharp
public class TurnOrderUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject portraitPrefab;

    public void UpdateTurnOrder(List<CombatCharacter> turnOrder)
    {
        // Clear existing
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // Create portraits in order
        foreach (var character in turnOrder)
        {
            var portrait = Instantiate(portraitPrefab, container);
            portrait.GetComponent<Image>().sprite = character.Portrait;
        }
    }
}
```

## ⚡ Performance Tips

```csharp
// Cache references
private CameraShake cameraShake;

void Awake()
{
    cameraShake = Camera.main.GetComponent<CameraShake>();
}

// Use object pooling for damage numbers
Queue<DamageNumber> damageNumberPool = new Queue<DamageNumber>();

DamageNumber GetPooledNumber()
{
    if (damageNumberPool.Count > 0)
        return damageNumberPool.Dequeue();

    return Instantiate(damageNumberPrefab);
}

void ReturnToPool(DamageNumber number)
{
    number.gameObject.SetActive(false);
    damageNumberPool.Enqueue(number);
}
```

## 🔍 Debugging

```csharp
// Log combat state
void Update()
{
    if (Input.GetKeyDown(KeyCode.F1))
    {
        Debug.Log($"Combat State: {turnManager.CurrentState}");
        Debug.Log($"Current Turn: {turnManager.CurrentCharacter?.CharacterName}");

        foreach (var character in turnManager.PlayerCharacters)
        {
            Debug.Log($"Player: {character.CharacterName} - HP: {character.Stats.CurrentHP}/{character.Stats.MaxHP}");
        }

        foreach (var character in turnManager.EnemyCharacters)
        {
            Debug.Log($"Enemy: {character.CharacterName} - HP: {character.Stats.CurrentHP}/{character.Stats.MaxHP}");
        }
    }
}
```

## 📱 Inspector Tips

### Character Setup Checklist
- ✅ Set character name
- ✅ Set Is Player checkbox
- ✅ Configure all stats (HP, MP, Attack, Defense, Speed)
- ✅ Assign Sprite Renderer
- ✅ Assign Combat Position transform
- ✅ Optional: Assign Animator

### TurnManager Setup
- ✅ Add player characters to Player Characters list
- ✅ Add enemy characters to Enemy Characters list
- ✅ Set Turn Delay (recommended: 0.5)

### CombatManager Setup
- ✅ Assign Turn Manager reference
- ✅ Assign Action Menu
- ✅ Assign Damage Number Spawner
- ✅ Assign all HUDs to respective lists
- ✅ Check Start On Awake if auto-starting

---

**Pro Tip:** Use Context Menu items (right-click component in Inspector) for quick testing!
