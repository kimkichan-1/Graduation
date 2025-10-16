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
        while (diceIndexA < pageA.diceList.Count && diceIndexB < pageB.diceList.Count)
        {
            CombatDice diceA = pageA.diceList[diceIndexA];
            CombatDice diceB = pageB.diceList[diceIndexB];

            Debug.Log($"--- [ {diceIndexA + 1}번째 주사위 합 ] ---");

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
                        int grossDamage = (characterA.attackPower + rollA) - characterB.defensePower;
                        if (diceB.type == DiceType.Defense)
                        {
                            int finalDamage = Mathf.Max(1, grossDamage - rollB);
                            Debug.Log($"{characterB.characterName}의 방어력({characterB.defensePower})과 수비 주사위({rollB})로 피해 경감! 최종 피해: {finalDamage}");
                            characterB.TakeDamage(finalDamage);
                        }
                        else
                        {
                            int finalDamage = Mathf.Max(1, grossDamage);
                            Debug.Log($"{characterB.characterName}의 방어력({characterB.defensePower})으로 피해 경감! 최종 피해: {finalDamage}");
                            characterB.TakeDamage(finalDamage);
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
                        int grossDamage = (characterB.attackPower + rollB) - characterA.defensePower;
                        if (diceA.type == DiceType.Defense)
                        {
                            int finalDamage = Mathf.Max(1, grossDamage - rollA);
                            Debug.Log($"{characterA.characterName}의 방어력({characterA.defensePower})과 수비 주사위({rollA})로 피해 경감! 최종 피해: {finalDamage}");
                            characterA.TakeDamage(finalDamage);
                        }
                        else
                        {
                            int finalDamage = Mathf.Max(1, grossDamage);
                            Debug.Log($"{characterA.characterName}의 방어력({characterA.defensePower})으로 피해 경감! 최종 피해: {finalDamage}");
                            characterA.TakeDamage(finalDamage);
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
                int finalDamage = Mathf.Max(1, (characterA.attackPower + roll) - characterB.defensePower);
                Debug.Log($"최종 피해: {finalDamage}");
                characterB.TakeDamage(finalDamage);
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
                int finalDamage = Mathf.Max(1, (characterB.attackPower + roll) - characterA.defensePower);
                Debug.Log($"최종 피해: {finalDamage}");
                characterA.TakeDamage(finalDamage);
            }
            diceIndexB++;
        }
        
        Debug.Log("-------------------------------------");
    }
}