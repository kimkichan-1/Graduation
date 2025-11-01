using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 포션 저장 데이터
/// </summary>
[System.Serializable]
public class PotionSaveData
{
    public string potionName;        // 포션 이름 (PotionItemData 식별용)
    public int quantity;             // 개수

    public PotionSaveData(string name, int qty)
    {
        potionName = name;
        quantity = qty;
    }
}

/// <summary>
/// 카드 저장 데이터
/// </summary>
[System.Serializable]
public class CardSaveData
{
    public string cardName;
    public int lightCost;
    public List<DiceSaveData> diceList = new List<DiceSaveData>();
    public string artworkResourcePath;  // 일러스트의 리소스 경로 또는 이름

    public CardSaveData() { }

    public CardSaveData(CombatPage card)
    {
        cardName = card.pageName;
        lightCost = card.lightCost;

        foreach (var dice in card.diceList)
        {
            diceList.Add(new DiceSaveData(dice));
        }

        // 일러스트가 있으면 이름만 저장 (나중에 매칭용)
        artworkResourcePath = card.artwork != null ? card.artwork.name : "";
    }

    /// <summary>
    /// CombatPage로 복원 (일러스트는 별도로 설정 필요)
    /// </summary>
    public CombatPage ToCombatPage()
    {
        CombatPage card = ScriptableObject.CreateInstance<CombatPage>();
        card.pageName = cardName;
        card.lightCost = lightCost;
        card.diceList = new List<CombatDice>();

        foreach (var diceSave in diceList)
        {
            card.diceList.Add(diceSave.ToCombatDice());
        }

        // 일러스트는 null로 초기화 (나중에 GameManager에서 설정)
        card.artwork = null;

        return card;
    }
}

/// <summary>
/// 주사위 저장 데이터
/// </summary>
[System.Serializable]
public class DiceSaveData
{
    public string diceType;  // "Attack" 또는 "Defense"
    public int minValue;
    public int maxValue;

    public DiceSaveData() { }

    public DiceSaveData(CombatDice dice)
    {
        diceType = dice.type.ToString();
        minValue = dice.minValue;
        maxValue = dice.maxValue;
    }

    public CombatDice ToCombatDice()
    {
        return new CombatDice
        {
            type = diceType == "Attack" ? DiceType.Attack : DiceType.Defense,
            minValue = minValue,
            maxValue = maxValue
        };
    }
}

/// <summary>
/// 게임 저장 데이터 구조
/// JSON으로 직렬화되어 파일에 저장됨
/// </summary>
[System.Serializable]
public class SaveData
{
    // ========== 메타 정보 ==========
    public string saveName = "New Save";           // 세이브 이름
    public string lastSaveTime;                    // 마지막 저장 시간
    public int slotNumber;                         // 슬롯 번호 (1, 2, 3)
    public bool isEmpty = true;                    // 빈 슬롯 여부
    public float playTime;                         // 총 플레이 시간 (초)

    // ========== 플레이어 기본 스탯 ==========
    public int playerLevel = 1;
    public int currentXp = 0;
    public int xpToNextLevel = 100;
    public int currentMoney = 0;
    public int currentHealth = 100;
    public int maxHealth = 100;

    // ========== 보너스 스탯 (영구 업그레이드) ==========
    public int bonusAttackPower = 0;
    public float bonusMoveSpeed = 0f;
    public int defense = 0;
    public int baseMaxHealth = 100;                // 기본 최대 체력

    // ========== 장착 무기 ==========
    public string equippedWeapon = "None";         // "Sword", "Mace", "Lance", "None"
    public bool hasSword = false;
    public bool hasMace = false;
    public bool hasLance = false;

    // ========== 위치 정보 ==========
    public string currentScene = "Stage1";         // 현재 씬 이름
    public float positionX = 0f;
    public float positionY = 0f;
    public float positionZ = 0f;

    // ========== 인벤토리 ==========
    public List<PotionSaveData> potionInventory = new List<PotionSaveData>();  // 포션 인벤토리

    // ========== 카드 시스템 ==========
    public List<CardSaveData> cardDeck = new List<CardSaveData>();           // 덱에 있는 카드들
    public List<CardSaveData> cardCollection = new List<CardSaveData>();     // 전체 소유 카드

    // ========== 게임 진행 상태 ==========
    public List<string> completedEvents = new List<string>();  // 완료한 이벤트 ID
    public List<string> usedPortals = new List<string>();      // 사용한 포탈 ID

    /// <summary>
    /// 새 세이브 데이터 생성 (초기화)
    /// </summary>
    public SaveData(int slot)
    {
        slotNumber = slot;
        saveName = $"Save {slot}";
        lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        isEmpty = false;
        playTime = 0f;
    }

    /// <summary>
    /// 빈 슬롯 데이터 생성
    /// </summary>
    public static SaveData CreateEmptySlot(int slot)
    {
        SaveData emptyData = new SaveData(slot);
        emptyData.isEmpty = true;
        emptyData.saveName = "Empty Slot";
        return emptyData;
    }

    /// <summary>
    /// 저장 시간 업데이트
    /// </summary>
    public void UpdateSaveTime()
    {
        lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 세이브 데이터 요약 정보 (UI 표시용)
    /// </summary>
    public string GetSummary()
    {
        if (isEmpty)
            return "Empty Slot";

        TimeSpan time = TimeSpan.FromSeconds(playTime);
        string playTimeStr = $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";

        return $"Lv.{playerLevel} | {currentScene} | {playTimeStr}\n{lastSaveTime}";
    }
}
