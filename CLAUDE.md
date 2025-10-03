# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a 2D action RPG game project called "Graduation" built with Unity 6000.0.41f1. The game features a multi-stage action platformer with weapon systems, inventory management, card mechanics, and RPG elements including health, stats, and level progression.

## Key Architecture

### Core Systems Architecture

**Player System**: Built around a modular weapon system with three weapon types (Sword, Lance, Mace), each providing different stats and abilities. The `PlayerController` (Assets/Scripts/Player/PlayerController.cs:5) manages movement, combat, and weapon switching with dynamic stat recalculation via `RecalculateStats()`. Features advanced movement including wall-sliding, dash mechanics with weapon-specific effects, stat-based speed modifications, and scene-based respawning via `OnSceneLoaded()` listener.

**Weapon System**: Each weapon type has its own attack script (`SwordAttack`, `LanceAttack`, `MaceAttack`) and weapon data (`WeaponStats`). Weapons affect player movement speed, dash properties, and combat abilities. The `WeaponSpawner` (Assets/Scripts/WeaponSpawner.cs:3) uses `GameData.SelectedWeapon` to dynamically instantiate the chosen weapon at scene start with precise positioning.

**UI & Inventory**: Comprehensive UI system including inventory management (`InventoryUI`, `InventorySlot`), stats display (`StatsUIManager`), and currency system. The `Inventory` (Assets/Scripts/UI/Inventory.cs:4) uses singleton pattern with delegate callbacks for real-time UI updates. Features tooltip system and slot-based item management.

**Card System**: Implements a card-based mechanic with `CardData` ScriptableObjects, draggable cards (`DraggableCard`), and card inventory management (`CardInventoryUI`).

**Stats & Progression**: Level-up system managed by `PlayerStats` (Assets/Scripts/Player/PlayerStats.cs:4) with XP tracking, currency system, and permanent stat bonuses. Features deferred level-up UI display for mini-game scenes (e.g., AncientBlacksmith) using `isLevelUpPending` flag. Stats are recalculated when weapons are equipped/unequipped.

**Scene Management**: Uses minimal `GameData` static class (Assets/Scripts/GameData.cs:3) for persistent weapon selection between scenes and `DontDestroyOnLoadManager` for cross-scene object persistence. Player positioning handled by `PlayerSpawnPoint` (Assets/Scripts/Player/PlayerSpawnPoint.cs) system with automatic scene loading integration via `SceneManager.sceneLoaded` events.

### Project Structure

- **Assets/Scripts/Player/**: Player controller, weapon systems, and player-related stats
- **Assets/Scripts/Monster/**: Enemy AI, health systems, spawning, and item drops
- **Assets/Scripts/UI/**: All UI components including inventory, stats, camera controls, and scene management
- **Assets/Scripts/CardSystem/**: Card-based gameplay mechanics
- **Assets/**: Scene files (Main.unity, Stage1.unity, Stage2.unity, etc.), sprites, and asset folders

### Key Scripts & Integration Points

- `PlayerController.cs:5`: Main player control with movement, combat, weapon management, and stat recalculation. Listens to scene load events for respawning.
- `PlayerStats.cs:4`: Manages level progression, XP, currency, and permanent stat bonuses with scene-aware level-up UI handling
- `GameData.cs:3`: Static class for cross-scene weapon selection persistence (minimal design - only stores `SelectedWeapon` string)
- `WeaponStats.cs:4`: Serializable class (not ScriptableObject) defining weapon properties as data structure
- `Inventory.cs:4`: Singleton inventory with delegate-based callback system for UI updates (`onInventoryChangedCallback`)
- `StatsUIManager.cs:4`: Centralizes stat display updates, handles both equipped and bare-handed states
- `WeaponSpawner.cs:3`: Instantiates selected weapon at scene start based on `GameData.SelectedWeapon`

## Development Commands

### Unity Editor Operations
- Open project in Unity 6000.0.41f1 or compatible version
- Build for Windows Standalone: File → Build Settings → PC, Mac & Linux Standalone
- Test in Play Mode using Unity Editor's Play button
- Run specific scenes: File → Build Settings → Add Open Scenes to build individual stages

### Version Control
```bash
git add .                    # Stage changes
git commit -m "message"      # Commit changes
git push origin main         # Push to main branch
```

### Unity Package Management
Packages are managed through Unity's Package Manager UI or by editing `Packages/manifest.json`. Current dependencies include:
- Unity 2D Feature Set (com.unity.feature.2d) - Sprite rendering, animation, physics
- AI Navigation (com.unity.ai.navigation) - Pathfinding for enemy AI
- Timeline (com.unity.timeline) - Cutscenes and scripted events
- Visual Scripting (com.unity.visualscripting) - Node-based scripting support
- Test Framework (com.unity.test-framework) - Unit and integration testing
- TextMeshPro (included via 2D features) - Advanced text rendering

## Development Guidelines

### Code Conventions
- Use Unity's C# coding conventions
- Public fields use PascalCase, private fields use camelCase
- Use `[Header("Section Name")]` for Inspector organization
- Implement proper null checks for component references
- Use `GetComponent<>()` calls in Start() or Awake() for performance

### Scene Organization
- Each stage has its own scene file (Stage1.unity, Stage2.unity)
- Main menu and settings are separate scenes
- Use PlayerSpawnPoint objects for player positioning across scenes
- Implement proper scene loading through GoScene scripts

### Asset Management
- Sprites organized in Assets/Sprites/ by category
- UI assets in dedicated folders (Dark_Brown_GUI_kit, etc.)
- Use Unity's Addressable system or direct references for asset loading
- Maintain consistent naming conventions for prefabs and ScriptableObjects

### Testing
- Test player movement and combat in each stage using Unity Play Mode
- Verify weapon switching and stat calculations work correctly via `RecalculateStats()` method
- Test inventory system functionality and UI responsiveness through delegate callbacks
- Ensure scene transitions and data persistence work properly with `GameData` and spawn points
- No automated test framework configured - manual testing required

## Common Tasks

### Adding New Weapons
1. Define weapon stats in a new `WeaponStats` data structure (it's a serializable class, not a ScriptableObject)
2. Implement weapon-specific attack script (e.g., `SwordAttack`, `LanceAttack`, `MaceAttack`)
3. Create weapon item script (e.g., `SwordItem`, `LanceItem`, `MaceItem`) for pickup/equip functionality
4. Add weapon prefab and update `WeaponSpawner` with new case in switch statement
5. Add weapon equip logic to `PlayerController` and ensure `RecalculateStats()` handles the new weapon
6. Update `StatsUIManager.cs:4` to display new weapon stats if needed

### Creating New Stages
1. Duplicate existing stage scene as template
2. Add PlayerSpawnPoint object for player positioning
3. Implement stage-specific monsters and challenges
4. Update scene loading scripts and build settings

### Modifying Player Stats
1. Add new stat fields to `PlayerStats.cs:4` (for permanent bonuses) or `WeaponStats.cs:4` (for weapon-specific stats)
2. Update `StatsUIManager.cs:4` UI fields and `UpdateStatsUI()` method to display the new stat
3. Modify `PlayerController.RecalculateStats()` to calculate the new stat (base + weapon + bonuses)
4. If adding a permanent upgradeable stat, add corresponding `Upgrade*()` method in `PlayerStats.cs:4`
5. Update `WeaponStats` data structures for each weapon to include the new stat values
