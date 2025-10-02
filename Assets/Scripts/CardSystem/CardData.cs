using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite artwork;
    // 여기에 비용, 공격력 등 다른 카드 속성을 추가할 수 있습니다.
}
