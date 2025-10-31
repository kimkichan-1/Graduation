using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 주사위 애니메이션을 관리하는 매니저
/// ClashManager와 함께 사용하여 시각적 연출을 담당합니다
/// </summary>
public class DiceAnimationManager : MonoBehaviour
{
    [Header("UI 위치")]
    [SerializeField] private Transform playerDiceContainer;
    [SerializeField] private Transform bossDiceContainer;
    [SerializeField] private Transform centerClashArea; // 합이 일어나는 중앙 영역

    [Header("프리팹")]
    [SerializeField] private GameObject diceVisualPrefab;

    [Header("애니메이션 타이밍")]
    [SerializeField] private float delayBetweenDice = 0.5f; // 주사위 사이 딜레이
    [SerializeField] private float delayAfterRoll = 0.8f; // 굴린 후 딜레이
    [SerializeField] private float delayAfterClash = 1.2f; // 합 후 딜레이

    [Header("사운드")]
    [SerializeField] private AudioClip clashSound; // 합 충돌 사운드
    [SerializeField] private AudioClip damageSound; // 데미지 입히는 사운드

    private AudioSource audioSource;
    private List<DiceVisual> playerDiceVisuals = new List<DiceVisual>();
    private List<DiceVisual> bossDiceVisuals = new List<DiceVisual>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// 카드의 주사위들을 시각적으로 생성
    /// </summary>
    public void SetupDiceVisuals(CombatPage playerPage, CombatPage bossPage)
    {
        // 기존 주사위 제거
        ClearDiceVisuals();

        // 플레이어 주사위 생성
        foreach (var dice in playerPage.diceList)
        {
            GameObject diceObj = Instantiate(diceVisualPrefab, playerDiceContainer);
            DiceVisual visual = diceObj.GetComponent<DiceVisual>();
            visual.Setup(dice.type, dice.minValue, dice.maxValue);
            playerDiceVisuals.Add(visual);
        }

        // 보스 주사위 생성
        foreach (var dice in bossPage.diceList)
        {
            GameObject diceObj = Instantiate(diceVisualPrefab, bossDiceContainer);
            DiceVisual visual = diceObj.GetComponent<DiceVisual>();
            visual.Setup(dice.type, dice.minValue, dice.maxValue);
            bossDiceVisuals.Add(visual);
        }
    }

    /// <summary>
    /// 주사위 합 애니메이션을 포함한 전체 클래시 시퀀스
    /// </summary>
    public IEnumerator AnimateClashSequence(
        CharacterStats playerStats, CombatPage playerPage,
        CharacterStats bossStats, CombatPage bossPage)
    {
        Debug.Log($"<color=#F1C40F><b>[애니메이션 시작]</b></color> {playerStats.characterName}의 '{playerPage.pageName}' vs {bossStats.characterName}의 '{bossPage.pageName}'");

        int diceIndexA = 0;
        int diceIndexB = 0;

        // 1단계: 합(Clash) 진행
        while (diceIndexA < playerPage.diceList.Count && diceIndexB < bossPage.diceList.Count)
        {
            CombatDice diceA = playerPage.diceList[diceIndexA];
            CombatDice diceB = bossPage.diceList[diceIndexB];

            Debug.Log($"--- [ {diceIndexA + 1}번째 주사위 합 ] ---");

            // 수비 vs 수비는 무승부
            if (diceA.type == DiceType.Defense && diceB.type == DiceType.Defense)
            {
                Debug.Log("<b>결과:</b> 수비 주사위끼리 맞붙어 <color=grey><b>무승부</b></color>");

                // 무승부 애니메이션
                if (diceIndexA < playerDiceVisuals.Count && diceIndexB < bossDiceVisuals.Count)
                {
                    yield return StartCoroutine(AnimateDrawClash(
                        playerDiceVisuals[diceIndexA],
                        bossDiceVisuals[diceIndexB]));
                }

                diceIndexA++;
                diceIndexB++;
                continue;
            }

            // 주사위 굴리기
            int rollA = diceA.Roll();
            int rollB = diceB.Roll();

            // 주사위 애니메이션 동시 실행
            if (diceIndexA < playerDiceVisuals.Count && diceIndexB < bossDiceVisuals.Count)
            {
                yield return StartCoroutine(AnimateDiceRoll(
                    playerDiceVisuals[diceIndexA],
                    bossDiceVisuals[diceIndexB],
                    rollA, rollB));
            }

            yield return new WaitForSeconds(delayAfterRoll);

            // 충돌 사운드
            if (clashSound != null)
            {
                audioSource.PlayOneShot(clashSound);
            }

            // 승패 판정 및 데미지 처리
            if (rollA > rollB)
            {
                Debug.Log($"<color=cyan><b>{playerStats.characterName} 승리!</b></color> ({rollA} vs {rollB})");

                // 승리 애니메이션
                if (diceIndexA < playerDiceVisuals.Count && diceIndexB < bossDiceVisuals.Count)
                {
                    StartCoroutine(playerDiceVisuals[diceIndexA].AnimateWin());
                    StartCoroutine(bossDiceVisuals[diceIndexB].AnimateLose());
                }

                // 데미지 처리
                yield return StartCoroutine(ApplyClashDamage(
                    playerStats, diceA, rollA, bossStats, diceB, rollB));
                
                // [수정됨] diceIndexB++ 삭제
            }
            else if (rollB > rollA)
            {
                Debug.Log($"<color=orange><b>{bossStats.characterName} 승리!</b></color> ({rollB} vs {rollA})");

                // 승리 애니메이션
                if (diceIndexA < playerDiceVisuals.Count && diceIndexB < bossDiceVisuals.Count)
                {
                    StartCoroutine(playerDiceVisuals[diceIndexA].AnimateLose());
                    StartCoroutine(bossDiceVisuals[diceIndexB].AnimateWin());
                }

                // 데미지 처리
                yield return StartCoroutine(ApplyClashDamage(
                    bossStats, diceB, rollB, playerStats, diceA, rollA));
                
                // [수정됨] diceIndexA++ 삭제
            }
            else
            {
                Debug.Log("<color=grey><b>무승부!</b></color>");

                // 무승부 애니메이션
                if (diceIndexA < playerDiceVisuals.Count && diceIndexB < bossDiceVisuals.Count)
                {
                    StartCoroutine(playerDiceVisuals[diceIndexA].AnimateDraw());
                    StartCoroutine(bossDiceVisuals[diceIndexB].AnimateDraw());
                }

                // [수정됨] diceIndexA++, diceIndexB++ 삭제
            }

            // ▼▼▼ [수정됨] 합을 한 번 진행한 주사위는 모두 사용된 것으로 처리 ▼▼▼
            diceIndexA++;
            diceIndexB++;
            // ▲▲▲

            yield return new WaitForSeconds(delayAfterClash);
        }

        // 2단계: 일방 공격
        Debug.Log("--- 합 종료, 남은 주사위로 일방 공격 ---");

        // 플레이어 남은 주사위
        while (diceIndexA < playerPage.diceList.Count)
        {
            yield return StartCoroutine(AnimateOneSidedAttack(
                playerDiceVisuals[diceIndexA],
                playerPage.diceList[diceIndexA],
                playerStats,
                bossStats));

            diceIndexA++;
            yield return new WaitForSeconds(delayBetweenDice);
        }

        // 보스 남은 주사위
        while (diceIndexB < bossPage.diceList.Count)
        {
            yield return StartCoroutine(AnimateOneSidedAttack(
                bossDiceVisuals[diceIndexB],
                bossPage.diceList[diceIndexB],
                bossStats,
                playerStats));

            diceIndexB++;
            yield return new WaitForSeconds(delayBetweenDice);
        }

        Debug.Log("-------------------------------------");

        // 모든 주사위 페이드아웃
        yield return StartCoroutine(FadeOutAllDice());
    }

    /// <summary>
    /// 두 주사위를 동시에 굴리는 애니메이션
    /// </summary>
    private IEnumerator AnimateDiceRoll(DiceVisual diceA, DiceVisual diceB, int valueA, int valueB)
    {
        Coroutine rollA = StartCoroutine(diceA.AnimateRoll(valueA));
        Coroutine rollB = StartCoroutine(diceB.AnimateRoll(valueB));

        // 두 애니메이션이 모두 끝날 때까지 대기
        yield return rollA;
        yield return rollB;
    }

    /// <summary>
    /// 무승부 애니메이션
    /// </summary>
    private IEnumerator AnimateDrawClash(DiceVisual diceA, DiceVisual diceB)
    {
        StartCoroutine(diceA.AnimateDraw());
        StartCoroutine(diceB.AnimateDraw());
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 합(Clash) 데미지 적용 - ClashManager의 로직 호출
    /// </summary>
    private IEnumerator ApplyClashDamage(
        CharacterStats winner, CombatDice winnerDice, int winnerRoll,
        CharacterStats loser, CombatDice loserDice, int loserRoll)
    {
        switch (winnerDice.type)
        {
            case DiceType.Attack:
                int finalDamage = ClashManager.CalculateAttackDamage(
                    winner, winnerRoll, loser, loserDice, loserRoll);
                
                Debug.Log($"{loser.characterName}의 방어력/수비로 피해 경감! 최종 피해: {finalDamage}");

                if (damageSound != null) audioSource.PlayOneShot(damageSound);
                loser.TakeDamage(finalDamage);
                break;

            case DiceType.Defense:
                int counterDamage = ClashManager.CalculateCounterDamage(winnerRoll, loserRoll);

                Debug.Log($"{winner.characterName}의 수비 성공! {loser.characterName}에게 {counterDamage}의 반격 피해를 줍니다.");

                if (damageSound != null) audioSource.PlayOneShot(damageSound);
                loser.TakeDamage(counterDamage);
                break;
        }

        yield return new WaitForSeconds(0.3f);
    }

    /// <summary>
    /// 일방 공격 애니메이션 - ClashManager의 로직 호출
    /// </summary>
    private IEnumerator AnimateOneSidedAttack(
        DiceVisual diceVisual, CombatDice dice,
        CharacterStats attacker, CharacterStats target)
    {
        int roll = dice.Roll();

        Debug.Log($"{attacker.characterName}의 남은 주사위로 일방 공격! ({dice.type}): {roll}");

        yield return StartCoroutine(diceVisual.AnimateRoll(roll));
        yield return new WaitForSeconds(0.3f);

        if (dice.type == DiceType.Attack)
        {
            int finalDamage = ClashManager.CalculateOneSidedAttackDamage(
                attacker, roll, target);
            
            Debug.Log($"최종 피해: {finalDamage}");

            if (damageSound != null)
            {
                audioSource.PlayOneShot(damageSound);
            }

            target.TakeDamage(finalDamage);
            StartCoroutine(diceVisual.AnimateWin());
        }

        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 모든 주사위 페이드아웃
    /// </summary>
    private IEnumerator FadeOutAllDice()
    {
        foreach (var dice in playerDiceVisuals)
        {
            if (dice != null)
            {
                StartCoroutine(dice.AnimateFadeOut());
            }
        }

        foreach (var dice in bossDiceVisuals)
        {
            if (dice != null)
            {
                StartCoroutine(dice.AnimateFadeOut());
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 주사위 비주얼 정리
    /// </summary>
    public void ClearDiceVisuals()
    {
        foreach (var dice in playerDiceVisuals)
        {
            if (dice != null) Destroy(dice.gameObject);
        }
        playerDiceVisuals.Clear();

        foreach (var dice in bossDiceVisuals)
        {
            if (dice != null) Destroy(dice.gameObject);
        }
        bossDiceVisuals.Clear();
    }

    private void OnDestroy()
    {
        ClearDiceVisuals();
    }
}