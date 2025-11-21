# 2D Turn-Based Combat System for Unity

A complete, production-ready turn-based combat system for Unity 2D games.

## 🎮 Features

- ✅ **Turn-Based Combat** - Speed-based initiative system
- ✅ **Character Stats** - HP, MP, Attack, Defense, Speed
- ✅ **Combat Actions** - Attack, Defend, Skills/Abilities
- ✅ **Status Effects** - Poison, Burn, Regen, Buffs, Debuffs, Stun
- ✅ **Ability System** - Flexible ScriptableObject-based skills
- ✅ **Combat UI** - Health bars, action menus, turn indicators
- ✅ **Visual Feedback** - Damage numbers, camera shake, particle effects
- ✅ **Event System** - Decoupled, extensible event-driven architecture
- ✅ **Simple AI** - Basic enemy decision making
- ✅ **Easy Setup** - Quick setup tools and comprehensive documentation

## 📁 Project Structure

```
Assets/
├── Scripts/Combat/
│   ├── Core/                 # Core combat logic
│   ├── Characters/           # Character and stats
│   ├── Abilities/            # Skill system
│   ├── Effects/              # Visual and status effects
│   └── UI/                   # User interface
├── Data/Abilities/           # Ability ScriptableObjects
├── Prefabs/Combat/           # Combat prefabs
└── Animations/Combat/        # Combat animations
```

## 🚀 Quick Start

### Method 1: Quick Setup (Fastest)

1. Create empty GameObject in your scene
2. Add `QuickCombatSetup` component
3. Press Play!
4. Use **SPACE** to attack, **D** to defend

### Method 2: Manual Setup (Full Control)

Follow the detailed [SETUP_GUIDE.md](Assets/Scripts/Combat/SETUP_GUIDE.md)

## 📖 Documentation

- **[README.md](Assets/Scripts/Combat/README.md)** - Full system documentation
- **[SETUP_GUIDE.md](Assets/Scripts/Combat/SETUP_GUIDE.md)** - Step-by-step scene setup
- Code comments - Comprehensive inline documentation

## 🎯 Core Systems

### Combat Flow

```
Battle Start → Turn Order → Player/Enemy Turns → Actions → Effects → Win/Loss Check
```

### Combat States

- `Idle` - Not in combat
- `BattleStart` - Initializing
- `PlayerTurn` - Player choosing action
- `PlayerAction` - Executing player action
- `EnemyTurn` - Enemy deciding
- `EnemyAction` - Executing enemy action
- `Processing` - Processing effects
- `Victory/Defeat` - Battle end

### Character Stats

| Stat | Description |
|------|-------------|
| **MaxHP** | Maximum health points |
| **MaxMP** | Maximum magic/skill points |
| **Attack** | Physical attack power |
| **Defense** | Damage reduction |
| **Speed** | Turn order priority |

### Status Effects

| Type | Effect |
|------|--------|
| **Poison/Burn** | Damage over time |
| **Regen** | Healing over time |
| **AttackUp/Down** | Modify attack stat |
| **DefenseUp/Down** | Modify defense stat |
| **Haste/Slow** | Modify speed stat |
| **Stun** | Cannot act (WIP) |

## 🎨 Customization

### Creating Abilities

1. Right-click in Project → Create → Combat → Ability
2. Configure properties:
   - Name, description, icon
   - MP cost, cooldown
   - Target type (single/all, enemy/ally)
   - Effect type (damage/heal/buff/debuff)
   - Power, multipliers
   - Optional status effects

### Example Abilities Included

- **Fireball** - Damage + chance to burn
- **Heal** - Restore HP to ally
- **Power Strike** - Strong physical attack

### Extending Characters

```csharp
public class CustomCharacter : CombatCharacter
{
    public override IEnumerator Attack(CombatCharacter target)
    {
        // Custom logic here
        yield return base.Attack(target);
    }
}
```

### Custom AI

Modify `TurnManager.EnemyTurnRoutine()` for custom enemy behavior.

## 🔧 Technical Details

### Architecture

- **Pattern**: MVC-like separation
- **Events**: C# events/UnityEvents for decoupling
- **Data**: ScriptableObjects for configuration
- **Coroutines**: For action sequences
- **State Machine**: For combat flow control

### Events System

#### TurnManager Events
- `OnStateChanged(CombatState)`
- `OnTurnStart(CombatCharacter)`
- `OnTurnEnd(CombatCharacter)`
- `OnBattleStart()`
- `OnBattleEnd()`

#### Character Events
- `OnDamageDealt(int)`
- `OnDamageTaken(int)`
- `OnHealed(int)`
- `OnDeath()`
- `OnActionComplete()`

## 🎮 Controls (Quick Setup Mode)

- **SPACE** - Attack (during player turn)
- **D** - Defend (during player turn)

## 💡 Best Practices

1. Use events for UI updates
2. Create abilities as ScriptableObjects
3. Check combat state before actions
4. Cache component references
5. Use object pooling for effects

## 🔨 Requirements

- Unity 2022.3 LTS or higher
- TextMeshPro package
- 2D packages (included in project)

## 📝 Included Scripts

### Core
- `CombatState.cs` - Combat state enum
- `TurnManager.cs` - Turn order and flow
- `CombatManager.cs` - Main coordinator
- `QuickCombatSetup.cs` - Quick test setup

### Characters
- `CombatCharacter.cs` - Base character class
- `CharacterStats.cs` - Stats container

### Abilities
- `CombatAbility.cs` - Ability ScriptableObject

### Effects
- `StatusEffect.cs` - Status effect system
- `CameraShake.cs` - Camera shake effect
- `CombatEffect.cs` - Particle effects

### UI
- `CombatHUD.cs` - Character health display
- `ActionMenu.cs` - Player action selection
- `DamageNumber.cs` - Floating damage text
- `DamageNumberSpawner.cs` - Damage number spawner

## 🚧 Future Enhancements

- [ ] Items and inventory
- [ ] Equipment system
- [ ] Character progression/leveling
- [ ] Multiple party members
- [ ] Formation system
- [ ] Combo attacks
- [ ] Elemental affinities
- [ ] Battle rewards
- [ ] Sound effects/music
- [ ] Save/load system

## 🐛 Troubleshooting

### Combat doesn't start
- Check `Start On Awake` in CombatManager
- Verify characters assigned to TurnManager

### UI not updating
- Check event subscriptions
- Verify component references
- Ensure stats are initialized

### No damage numbers
- Assign prefab to DamageNumberSpawner
- Check Canvas reference
- Verify Main Camera tag

See [SETUP_GUIDE.md](Assets/Scripts/Combat/SETUP_GUIDE.md) for more solutions.

## 📄 License

This system is provided as-is for use in your Unity projects.

## 🙏 Credits

Created with Unity 2022.3 LTS using C# and Unity's 2D packages.

---

**Ready to create your turn-based RPG?** Start with Quick Setup or follow the Setup Guide! 🎮✨
