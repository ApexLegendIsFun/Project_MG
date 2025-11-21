# Combat System Architecture

## System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         COMBAT SYSTEM                            │
│                                                                   │
│  ┌──────────────┐      ┌──────────────┐      ┌──────────────┐  │
│  │   UI Layer   │◄────►│  Core Logic  │◄────►│  Data Layer  │  │
│  └──────────────┘      └──────────────┘      └──────────────┘  │
│         │                      │                      │          │
│         │                      │                      │          │
│         ▼                      ▼                      ▼          │
│  ┌──────────────┐      ┌──────────────┐      ┌──────────────┐  │
│  │  Effects &   │      │  Characters  │      │  Abilities   │  │
│  │   Feedback   │      │   & Stats    │      │ (ScriptObjs) │  │
│  └──────────────┘      └──────────────┘      └──────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

## Component Relationships

```
CombatManager (Main Coordinator)
    │
    ├─► TurnManager (Turn Order & State)
    │       ├─► PlayerCharacters[]
    │       │       └─► CombatCharacter
    │       │               ├─► CharacterStats
    │       │               ├─► StatusEffect[]
    │       │               └─► Abilities[]
    │       │
    │       └─► EnemyCharacters[]
    │               └─► CombatCharacter (same as above)
    │
    ├─► ActionMenu (Player Input)
    │       ├─► Attack Button
    │       ├─► Skills Button
    │       ├─► Defend Button
    │       └─► Flee Button
    │
    ├─► CombatHUD[] (Display)
    │       ├─► HP Slider
    │       ├─► MP Slider
    │       ├─► Name Text
    │       └─► Turn Indicator
    │
    └─► DamageNumberSpawner (Feedback)
            └─► DamageNumber Prefab
```

## Combat Flow State Machine

```
        START
          │
          ▼
    ┌──────────┐
    │   IDLE   │
    └──────────┘
          │
          │ StartBattle()
          ▼
    ┌──────────────┐
    │ BATTLE_START │
    └──────────────┘
          │
          │ Calculate Turn Order
          ▼
    ┌──────────────┐
    │  PLAYER_TURN │◄───────┐
    └──────────────┘        │
          │                 │
          │ Select Action   │
          ▼                 │
    ┌──────────────┐        │
    │PLAYER_ACTION │        │
    └──────────────┘        │
          │                 │
          ▼                 │
    ┌──────────────┐        │
    │ PROCESSING   │        │
    └──────────────┘        │
          │                 │
          ├─ Check Win? ────┤
          │                 │
          ▼                 │
    ┌──────────────┐        │
    │  ENEMY_TURN  │        │
    └──────────────┘        │
          │                 │
          │ AI Decision     │
          ▼                 │
    ┌──────────────┐        │
    │ ENEMY_ACTION │        │
    └──────────────┘        │
          │                 │
          ▼                 │
    ┌──────────────┐        │
    │ PROCESSING   │        │
    └──────────────┘        │
          │                 │
          ├─ Check Win? ────┘
          │
          ├─► VICTORY
          │
          └─► DEFEAT
```

## Event Flow

```
User Input
    │
    ▼
ActionMenu
    │
    │ OnButtonClick
    ▼
TurnManager.ExecutePlayerAction()
    │
    ├─► Change State: PLAYER_ACTION
    │
    ├─► Execute Coroutine (Attack/Defend/Skill)
    │       │
    │       ├─► CombatCharacter.Attack(target)
    │       │       │
    │       │       ├─► Play Animation
    │       │       │
    │       │       ├─► Calculate Damage
    │       │       │
    │       │       └─► target.TakeDamage()
    │       │               │
    │       │               ├─► CharacterStats.TakeDamage()
    │       │               │       │
    │       │               │       └─► OnHPChanged Event
    │       │               │               │
    │       │               │               └─► CombatHUD.UpdateHP()
    │       │               │
    │       │               ├─► OnDamageTaken Event
    │       │               │       │
    │       │               │       └─► DamageNumberSpawner.SpawnDamage()
    │       │               │
    │       │               └─► Flash Sprite Effect
    │       │
    │       └─► OnActionComplete Event
    │
    ├─► Change State: PROCESSING
    │
    ├─► Process Status Effects
    │
    ├─► Check Win/Loss
    │
    └─► Next Turn
```

## Class Hierarchy

```
MonoBehaviour
    │
    ├─► CombatManager
    │
    ├─► TurnManager
    │
    ├─► CombatCharacter
    │       │
    │       └─► [Your Custom Character Classes]
    │               ├─► PlayerCharacter
    │               ├─► MageCharacter
    │               └─► WarriorCharacter
    │
    ├─► CombatHUD
    │
    ├─► ActionMenu
    │
    ├─► DamageNumber
    │
    ├─► DamageNumberSpawner
    │
    ├─► CameraShake
    │
    └─► CombatEffect

ScriptableObject
    │
    └─► CombatAbility
            ├─► Fireball
            ├─► Heal
            └─► PowerStrike

[Serializable] Classes
    │
    ├─► CharacterStats
    │
    └─► StatusEffect

Enums
    │
    ├─► CombatState
    ├─► StatusEffectType
    ├─► AbilityEffectType
    ├─► TargetType
    └─► StatType
```

## Data Flow: Attack Action

```
1. User clicks "Attack" button
   │
   ▼
2. ActionMenu.OnAttackClicked()
   │
   ▼
3. ActionMenu.ShowTargetSelection()
   │
   ▼
4. ActionMenu.PerformAttack(target)
   │
   ▼
5. TurnManager.ExecutePlayerAction(target, attackAction)
   │
   ▼
6. TurnManager changes state to PLAYER_ACTION
   │
   ▼
7. Start Coroutine: CombatCharacter.Attack(target)
   │
   ├─► Play attack animation
   │
   ├─► Wait 0.3s
   │
   ├─► Calculate damage = CalculateDamage(stats.Attack)
   │
   ├─► actualDamage = target.TakeDamage(damage)
   │       │
   │       ├─► actualDamage = CharacterStats.TakeDamage(damage)
   │       │       │
   │       │       ├─► actualDamage = max(0, damage - defense)
   │       │       │
   │       │       ├─► currentHP -= actualDamage
   │       │       │
   │       │       ├─► Fire OnHPChanged event
   │       │       │       │
   │       │       │       └─► CombatHUD receives event
   │       │       │               │
   │       │       │               └─► Updates HP slider & text
   │       │       │
   │       │       └─► If HP <= 0: Fire OnDeath event
   │       │
   │       ├─► Play hit animation
   │       │
   │       ├─► Start FlashSprite coroutine
   │       │
   │       └─► Fire OnDamageTaken event
   │               │
   │               └─► DamageNumberSpawner receives event
   │                       │
   │                       └─► Spawns floating damage number
   │
   ├─► Fire OnDamageDealt event
   │
   ├─► Wait 0.3s
   │
   ├─► Return to idle animation
   │
   └─► Fire OnActionComplete event
       │
       ▼
8. TurnManager.ExecuteActionRoutine continues
   │
   ├─► Wait 0.5s
   │
   └─► Call NextTurn()
       │
       ▼
9. Change state to PROCESSING
   │
   ▼
10. Process status effects
    │
    ▼
11. Check victory/defeat conditions
    │
    ▼
12. Next character's turn begins
```

## Event System Architecture

```
Unity Events (used for Inspector wiring)
    │
    ├─► TurnManager
    │       ├─► OnStateChanged (CombatState)
    │       ├─► OnTurnStart (CombatCharacter)
    │       ├─► OnTurnEnd (CombatCharacter)
    │       ├─► OnBattleStart ()
    │       └─► OnBattleEnd ()
    │
    └─► CombatCharacter
            ├─► OnDamageDealt (int)
            ├─► OnDamageTaken (int)
            ├─► OnHealed (int)
            ├─► OnDeath ()
            └─► OnActionComplete ()

C# Events (used for internal communication)
    │
    └─► CharacterStats
            ├─► OnHPChanged (int current, int max)
            ├─► OnMPChanged (int current, int max)
            └─► OnDeath ()
```

## Dependency Injection Pattern

```
CombatManager (Top Level)
    │
    │ Injects references into:
    │
    ├─► ActionMenu
    │       └─► Needs: TurnManager
    │
    ├─► CombatHUD
    │       └─► Needs: CombatCharacter
    │
    └─► DamageNumberSpawner
            └─► Needs: Canvas, DamageNumber Prefab
```

## Ability Execution Flow

```
1. User selects ability from Skills menu
   │
   ▼
2. ActionMenu.OnSkillSelected(ability)
   │
   ├─► Check: ability.CanUse(character)
   │       │
   │       └─► Verify: currentMP >= mpCost
   │
   ▼
3. ActionMenu.ShowTargetSelection(performSkillCallback)
   │
   ▼
4. ActionMenu.PerformSkill(ability, target)
   │
   ▼
5. TurnManager.ExecutePlayerAction(target, skillAction)
   │
   ▼
6. Start Coroutine: CombatAbility.Execute(user, target)
   │
   ├─► Consume MP: user.Stats.ConsumeMP(mpCost)
   │
   ├─► Spawn visual effect at target position
   │
   ├─► Wait 0.3s
   │
   ├─► ApplyEffect(user, target)
   │       │
   │       ├─► Calculate power based on effect type
   │       │
   │       ├─► Switch on effectType:
   │       │   │
   │       │   ├─► Damage: target.TakeDamage(power)
   │       │   ├─► Heal: target.Heal(power)
   │       │   ├─► Buff: target.AddStatusEffect(effect)
   │       │   └─► Debuff: target.AddStatusEffect(effect)
   │       │
   │       └─► If appliesStatusEffect && random chance:
   │               └─► target.AddStatusEffect(statusEffect)
   │
   └─► Wait 0.3s
```

## Status Effect Processing

```
Each Turn in PROCESSING state:
    │
    ▼
TurnManager.ProcessEffects()
    │
    │ For each character in turnOrder:
    │
    └─► CombatCharacter.ProcessStatusEffects()
            │
            │ For each effect in activeEffects:
            │
            └─► StatusEffect.Process(character)
                    │
                    ├─► Switch on effectType:
                    │   │
                    │   ├─► Poison/Burn:
                    │   │   └─► character.TakeDamage(power)
                    │   │
                    │   ├─► Regen:
                    │   │   └─► character.Heal(power)
                    │   │
                    │   └─► Stun:
                    │       └─► Log stun message
                    │
                    ├─► Decrement turnsRemaining
                    │
                    └─► If expired:
                            └─► RemoveStatusEffect(effect)
                                    │
                                    └─► Reverse stat modifications
```

## Turn Order Calculation

```
TurnManager.CalculateTurnOrder()
    │
    ├─► Clear turnOrder list
    │
    ├─► Add all playerCharacters to list
    │
    ├─► Add all enemyCharacters to list
    │
    ├─► Sort by Speed stat (descending)
    │       │
    │       └─► Higher speed = earlier turn
    │
    └─► Reset currentTurnIndex to 0

Example:
    Characters:
    - Hero (Speed: 12)
    - Mage (Speed: 8)
    - Goblin (Speed: 10)
    - Orc (Speed: 6)

    Turn Order:
    1. Hero (12)
    2. Goblin (10)
    3. Mage (8)
    4. Orc (6)
    (Loop back to 1)
```

## Memory Management

```
Object Lifecycle:
    │
    ├─► CombatManager: Persistent (scene lifetime)
    │
    ├─► TurnManager: Persistent (scene lifetime)
    │
    ├─► CombatCharacter: Persistent until death
    │       └─► On death: Fade out → Can be pooled or destroyed
    │
    ├─► StatusEffect: Temporary (duration-based)
    │       └─► Removed when turnsRemaining <= 0
    │
    ├─► DamageNumber: Temporary (1.5s lifetime)
    │       └─► Auto-destroyed after animation
    │       └─► Recommended: Use object pooling
    │
    └─► CombatEffect: Temporary (until particles complete)
            └─► Auto-destroyed when not alive
```

## Extensibility Points

```
To Add New Features:
    │
    ├─► New Character Type:
    │       └─► Inherit from CombatCharacter
    │           └─► Override Attack(), Defend(), etc.
    │
    ├─► New Ability:
    │       └─► Create new CombatAbility ScriptableObject
    │           └─► Configure in Inspector
    │
    ├─► New Status Effect:
    │       └─► Add to StatusEffectType enum
    │           └─► Add case in StatusEffect.Apply/Process/Remove
    │
    ├─► New AI Behavior:
    │       └─► Modify TurnManager.EnemyTurnRoutine()
    │           └─► Implement custom decision logic
    │
    ├─► New UI Element:
    │       └─► Subscribe to relevant events
    │           └─► Update on event callbacks
    │
    └─► New Visual Effect:
            └─► Create prefab with CombatEffect component
                └─► Assign to ability or create manually
```

---

## Key Design Principles

1. **Separation of Concerns**: UI, Logic, and Data are separate
2. **Event-Driven**: Loose coupling via events
3. **Data-Driven**: Abilities and stats in ScriptableObjects
4. **State Machine**: Clear combat flow control
5. **Extensibility**: Easy to add new features
6. **Modularity**: Components work independently
7. **Testability**: Each system can be tested separately

This architecture supports:
- Easy testing and debugging
- Simple feature additions
- Clear code organization
- Maintainable codebase
- Performance optimization opportunities
