using UnityEngine;

/// <summary>
/// 상점에서 판매되는 아이템 데이터
/// </summary>
[CreateAssetMenu(fileName = "New ShopItem", menuName = "Shop/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    public enum ItemType
    {
        Potion,              // 체력 포션 (인벤토리 추가)
        AttackUpgrade,       // 공격력 영구 증가
        DefenseUpgrade,      // 방어력 영구 증가
        HealthUpgrade,       // 최대 체력 영구 증가
        SpeedUpgrade         // 이동속도 영구 증가
    }

    [Header("기본 정보")]
    public string itemName = "New Item";
    [TextArea(3, 5)]
    public string description = "Item description";
    public Sprite icon;
    public int price = 100;

    [Header("아이템 타입 및 효과")]
    public ItemType itemType;
    public int effectValue;  // 체력 회복량, 스탯 증가량 등

    [Header("포션 전용 (ItemType이 Potion일 때만 사용)")]
    public PotionItemData potionData;  // 실제 사용할 포션 데이터

    [Header("아이콘 크기 설정")]
    [Tooltip("아이콘 크기 배율 (1.0 = 기본 크기)")]
    public float iconScale = 1.0f;

    /// <summary>
    /// 아이템 효과 적용
    /// </summary>
    public void ApplyEffect(PlayerController player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        switch (itemType)
        {
            case ItemType.Potion:
                // 포션은 인벤토리에 추가
                if (potionData != null && Inventory.instance != null)
                {
                    Inventory.instance.Add(potionData);
                    Debug.Log($"{itemName} 포션을 인벤토리에 추가했습니다.");
                }
                break;

            case ItemType.AttackUpgrade:
                if (stats != null)
                {
                    stats.bonusAttackPower += effectValue;
                    player.RecalculateStats();
                    player.UpdateAllStatsUI();
                    Debug.Log($"공격력이 {effectValue} 증가했습니다! (총 보너스: {stats.bonusAttackPower})");
                }
                break;

            case ItemType.DefenseUpgrade:
                if (health != null)
                {
                    health.defense += effectValue;
                    Debug.Log($"방어력이 {effectValue} 증가했습니다! (현재 방어력: {health.defense})");
                }
                break;

            case ItemType.HealthUpgrade:
                if (health != null)
                {
                    health.AddPermanentHealth(effectValue);
                    Debug.Log($"최대 체력이 {effectValue} 증가했습니다!");
                }
                break;

            case ItemType.SpeedUpgrade:
                if (stats != null)
                {
                    stats.bonusMoveSpeed += (float)effectValue;
                    player.RecalculateStats();
                    player.UpdateAllStatsUI();
                    Debug.Log($"이동속도가 {effectValue} 증가했습니다! (총 보너스: {stats.bonusMoveSpeed})");
                }
                break;
        }
    }
}
