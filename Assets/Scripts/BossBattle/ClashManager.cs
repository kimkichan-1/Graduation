using UnityEngine;

public class ClashManager
{
    public static void ResolveClash(CharacterStats characterA, CombatPage pageA, CharacterStats characterB, CombatPage pageB)
    {
        // --- 카드 대결 시작 로그 ---
        Debug.Log($"<color=#F1C40F><b>[카드 대결 시작]</b></color> {characterA.characterName}의 <b>'{pageA.pageName}'</b> vs {characterB.characterName}의 <b>'{pageB.pageName}'</b>");

        int diceIndexA = 0;
        int diceIndexB = 0;

        // --- 1단계: 합(Clash) 진행 ---
        // 양쪽 모두에게 굴릴 주사위가 남아있는 동안 합을 반복
        while (diceIndexA < pageA.diceList.Count && diceIndexB < pageB.diceList.Count)
        {
            CombatDice diceA = pageA.diceList[diceIndexA];
            CombatDice diceB = pageB.diceList[diceIndexB];

            Debug.Log($"--- [ {diceIndexA + 1}번째 주사위 합 ] ---");

            // 규칙: 수비 주사위끼리 합하면 무조건 무승부
            if (diceA.type == DiceType.Defense && diceB.type == DiceType.Defense)
            {
                Debug.Log("<b>결과:</b> 수비 주사위끼리 맞붙어 <color=grey><b>무승부</b></color> 처리됩니다.");
                diceIndexA++;
                diceIndexB++;
                continue; 
            }
            
            int rollA = diceA.Roll();
            int rollB = diceB.Roll();

            string logMessage = $"<b>{characterA.characterName}</b>의 <b>{diceA.type}</b> 주사위 ({diceA.minValue}-{diceA.maxValue}): <color=white><b>{rollA}</b></color>\n" +
                                $"<b>{characterB.characterName}</b>의 <b>{diceB.type}</b> 주사위 ({diceB.minValue}-{diceB.maxValue}): <color=white><b>{rollB}</b></color>\n";

            if (rollA > rollB) // A가 승리
            {
                logMessage += $"<b>결과:</b> <color=cyan><b>{characterA.characterName} 승리!</b></color>";
                Debug.Log(logMessage);
                
                switch (diceA.type)
                {
                    case DiceType.Attack:
                        if (diceB.type == DiceType.Defense)
                        {
                            int damage = Mathf.Max(1, rollA - rollB);
                            Debug.Log($"{characterB.characterName}의 수비로 피해가 {rollB}만큼 경감됩니다.");
                            characterB.TakeDamage(damage);
                        }
                        else
                        {
                            characterB.TakeDamage(rollA);
                        }
                        break;
                    
                    case DiceType.Defense:
                        int counterDamage = rollA - rollB;
                        Debug.Log($"{characterA.characterName}의 수비 성공! {characterB.characterName}에게 {counterDamage}의 반격 피해를 줍니다.");
                        characterB.TakeDamage(counterDamage);
                        break;
                }
            }
            else if (rollB > rollA) // B가 승리
            {
                logMessage += $"<b>결과:</b> <color=orange><b>{characterB.characterName} 승리!</b></color>";
                Debug.Log(logMessage);
                
                switch (diceB.type)
                {
                    case DiceType.Attack:
                        if (diceA.type == DiceType.Defense)
                        {
                            int damage = Mathf.Max(1, rollB - rollA);
                            Debug.Log($"{characterA.characterName}의 수비로 피해가 {rollA}만큼 경감됩니다.");
                            characterA.TakeDamage(damage);
                        }
                        else
                        {
                            characterA.TakeDamage(rollB);
                        }
                        break;
                    
                    case DiceType.Defense:
                        int counterDamage = rollB - rollA;
                        Debug.Log($"{characterB.characterName}의 수비 성공! {characterA.characterName}에게 {counterDamage}의 반격 피해를 줍니다.");
                        characterA.TakeDamage(counterDamage);
                        break;
                }
            }
            else // 무승부
            {
                logMessage += "<b>결과:</b> <color=grey><b>무승부!</b></color>";
                Debug.Log(logMessage);
            }

            // 규칙: 승패와 상관없이, 합을 한 번 진행한 주사위는 모두 사용된 것으로 처리
            diceIndexA++;
            diceIndexB++;
        }

        // --- 2단계: 일방 공격 진행 ---
        Debug.Log("--- 합 종료, 남은 주사위로 일방 공격 ---");

        while (diceIndexA < pageA.diceList.Count)
        {
            CombatDice remainingDiceA = pageA.diceList[diceIndexA];
            int roll = remainingDiceA.Roll();
            Debug.Log($"{characterA.characterName}의 남은 <b>'{pageA.pageName}'</b> 카드의 {diceIndexA + 1}번째 주사위로 일방 공격! ({remainingDiceA.type}): {roll}");
            
            if (remainingDiceA.type == DiceType.Attack)
            {
                characterB.TakeDamage(roll);
            }
            diceIndexA++;
        }

        while (diceIndexB < pageB.diceList.Count)
        {
            CombatDice remainingDiceB = pageB.diceList[diceIndexB];
            int roll = remainingDiceB.Roll();
            Debug.Log($"{characterB.characterName}의 남은 <b>'{pageB.pageName}'</b> 카드의 {diceIndexB + 1}번째 주사위로 일방 공격! ({remainingDiceB.type}): {roll}");

            if (remainingDiceB.type == DiceType.Attack)
            {
                characterA.TakeDamage(roll);
            }
            diceIndexB++;
        }
        
        Debug.Log("-------------------------------------");
    }
}