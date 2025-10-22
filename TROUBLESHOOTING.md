# Troubleshooting Guide

이 문서는 Unity 2D Action RPG 프로젝트의 일반적인 문제 해결 방법을 다룹니다.

## Level-Up UI Not Showing
- Check if scene name is "AncientBlacksmith" (UI is intentionally suppressed there)
- Verify `LevelUpUIManager` is assigned in PlayerStats inspector
- Check Time.timeScale is being set to 0f
- Ensure PlayerStats.ShowPendingLevelUpPanel() is called when returning from minigame

## Portal Not Working
- Ensure `returnSpawnPoint` is assigned in PortalController inspector
- Check UI panel names match Hierarchy (e.g., "ConfirmationPanel", "ConfirmButton", "CancelButton")
- Verify target scene is in Build Settings
- Check portal hasn't been used already (usedPortalIDs list)

## Player Not Spawning in Correct Location
- Verify PlayerSpawnPoint exists in the scene
- Check PlayerController's OnSceneLoaded() is subscribed to SceneManager.sceneLoaded event
- For portal returns, verify PortalReturnData.hasReturnInfo is set correctly

## Damage Not Registering
- Check LayerMask settings in inspector (usually "Enemy" layer for monsters)
- Verify attack hitbox collider is set to trigger
- Ensure animator events call `EnableAttackHitbox()`/`DisableAttackHitbox()` at correct frames
- Check weapon-specific damage multipliers in MonsterHealth scripts

## Monster Not Moving
- Check if MonsterHealth and Animator are present
- Verify wall/ledge check transforms are assigned
- Confirm groundLayer is set correctly
- Ensure Rigidbody2D is present and not kinematic

## Boss Battle System Not Working
**For Dice Battle System:**
- Verify BossGameManager.Instance is not null and currentState transitions to Battle
- Check BattleController is present and has references to player/boss CharacterStats
- Ensure both player and boss have CharacterStats component with configured decks
- Verify CombatPage ScriptableObjects have proper CombatDice arrays
- Check BattleTrigger or BossAreaTrigger is configured to start battle
- Confirm camera controller and dice animation manager are assigned

**For Simple Turn-Based System:**
- Check that TurnManager is present in the scene
- Ensure ActionData ScriptableObject is assigned to PlayerController
- Confirm weapon action scripts (Sword.cs, Mace.cs, Lance.cs) are attached to player

## Item Drops Not Appearing
- Verify ItemDrop component is attached to monster GameObject
- Check drop prefabs are assigned in inspector (coinPrefab, potionPrefab, etc.)
- Ensure drop rates and quantities are configured properly
- Confirm ItemDrop.GenerateDrops() is called in MonsterHealth.DieRoutine()
- Check that coin/item prefabs have proper Collider2D (trigger) and Rigidbody2D components

## Shop Not Working
- Verify ShopManager is present in Shop scene and has `allAvailableItems` list populated (minimum 4 items)
- Check ShopPedestal references are assigned in ShopManager's `itemPedestals` array
- Ensure PlayerController.Instance is accessible (player persists via DontDestroyOnLoad)
- Verify ShopItemData ScriptableObjects have proper configuration:
  - For Potion type: PotionItemData reference must be assigned
  - effectValue should be set for stat upgrade types
- Check player has sufficient money via PlayerStats.currentMoney
- Verify AudioSource component exists on ShopManager for sound effects
- Confirm ShopPedestal.SetItem() is called during initialization

## Save/Load Not Working
**Save Issues:**
- Verify GameManager.Instance and SaveManager.Instance are present (check DontDestroyOnLoad)
- Confirm GameManager.currentSaveSlot is set (1-3 range)
- Check that PlayerController, PlayerStats, PlayerHealth, and Inventory singletons are accessible
- Verify save folder exists: Check `Application.persistentDataPath/Saves/`
- Use SaveManager's `[ContextMenu] Show Save Folder Path` to open save directory
- Ensure scene is not in excluded list (Main, LoadGame, Weapon, HowToPlay, Setting)
- Check console for save success message with file path

**Load Issues:**
- Verify save file exists (SaveSlot1.json, SaveSlot2.json, or SaveSlot3.json)
- Check JSON file is valid (not corrupted)
- Ensure target scene exists in Build Settings
- Verify PlayerSpawnPoint exists in target scene or savedSpawnPosition is valid
- Check that all singletons (PlayerController, Inventory) are properly initialized before load

**Auto-Save Issues:**
- Verify PlayerController.OnSceneLoaded() is subscribed to SceneManager.sceneLoaded event
- Check 0.5s delay is completing (no scene unload during delay)
- Confirm PlayerSpawnPoint exists in scene for position saving

## Stage3 (Underwater) Not Working
- Verify Stage3Manager is present in Stage3 scene
- Check PlayerSwimming and PlayerOxygen components are attached to player GameObject
- Confirm player has "Player" tag
- Ensure PlayerSwimming.enabled and PlayerOxygen.enabled are false by default
- Verify OxygenZone triggers have proper collider configuration
- Check that oxygenSlider UI reference is assigned in PlayerOxygen inspector
- Confirm Stage3Manager.OnDestroy() properly disables components when leaving scene

## Dice Animation Not Working
- Verify DiceVisual prefab has proper structure (Background Image, ValueText TextMeshProUGUI)
- Check DiceAnimationManager has references to playerDiceContainer and bossDiceContainer
- Ensure containers have Horizontal Layout Group component
- Confirm DiceVisual prefab is assigned in DiceAnimationManager
- Verify BattleController has diceAnimationManager reference assigned
- Check that CombatPage ScriptableObjects have CombatDice arrays populated
- Confirm AudioSource components exist for sound effects

## Pause Menu Not Working
- Verify PauseMenuUI component is attached to a Canvas in the scene
- Check pauseMenuPanel is assigned in inspector
- Ensure buttons (closeButton, settingButton, goMainButton) are assigned
- Confirm DontDestroyOnLoadManager exists on UI objects that should persist
- Check Time.timeScale is being set to 0f on pause
- Verify ESC key input is not blocked by other UI elements

## Main Menu Return Issues
**Player not destroyed when returning to Main:**
- Check PauseMenuUI.GoMainCoroutine() sets DontDestroyOnLoadManager.isReturningToMainMenu = true
- Verify PlayerController.Instance and Inventory.instance are set to null before scene load
- Confirm MainMenuController.CleanupDontDestroyOnLoadObjects() is running in Main scene

**DontDestroyOnLoad objects duplicating:**
- Ensure each DontDestroyOnLoadManager has unique instanceId set in inspector
- Check DontDestroyOnLoadManager.ResetMainMenuFlag() is called in MainMenuController
- Verify GameManager and SaveManager are preserved in cleanup (name check)

## New Game / Load Game Flow Issues
**New Game not starting:**
- Verify flow: Main → OnNewGameButton() → Weapon scene → WeaponChoice → LoadGame scene
- Check GameManager.isNewGame is set to true in WeaponChoice.Start()
- Confirm selectedWeapon is stored in GameData.SelectedWeapon and GameManager.selectedWeapon

**Load Game not working:**
- Verify flow: Main → OnLoadGameButton() → LoadGame scene
- Check GameManager.isNewGame is set to false
- Confirm SaveManager.Instance.GetAllSaveSlots() returns valid data
- Ensure LoadGameUI.OnSlotSelected() properly distinguishes empty vs filled slots

**Overwrite confirmation not showing:**
- Check overwriteConfirmPanel is assigned in LoadGameUI inspector
- Verify confirmOverwriteButton and cancelOverwriteButton are assigned
- Confirm ShowOverwriteConfirmation() is called when selecting filled slot in NewGame flow
