using UnityEngine;
using System.Collections;

// Mace.cs는 플레이어가 검을 장착했을 때 공격을 처리하는 스크립트입니다.
// 단일 공격과 2연속 콤보를 처리하며 애니메이션, 콤보 타이밍, 이동 제한, 사운드 등을 제어합니다.

public class Mace : MonoBehaviour
{
    // 컴포넌트 참조
    private PlayerController playerController; // 플레이어 상태를 확인하기 위한 참조
    private Animator animator;                 // 애니메이션 실행을 위한 참조
    private Rigidbody2D rb;                   // 공격 중 이동 제어를 위한 참조
    private AudioSource audioSource;          // 사운드 재생을 위한 참조

    // 공격 상태 변수
    private bool isAttacking = false;          // 현재 공격 중인지 여부
    private bool canCombo = false;             // 콤보 공격 가능 여부
    private float comboTimer = 0f;             // 콤보 입력 가능 시간 타이머
    private float comboDuration = 0.6f;        // 콤보 입력 가능 시간 (초)
    private int currentAttack = 0;             // 현재 공격 단계 (0=대기, 1=Attack1, 2=Attack2)
    private bool attackLocked = false;         // 공격 중 새로운 입력 방지 플래그

    // 사운드 클립
    [SerializeField] private AudioClip Maceattack1Sound; // Attack1 사운드 클립
    [SerializeField] private AudioClip Maceattack2Sound; // Attack2 사운드 클립

    // 초기화
    private void Start()
    {
        // 필요한 컴포넌트들을 가져옴
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // AudioSource가 없으면 추가
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 매 프레임 호출
    private void Update()
    {
        // 메이스가 장착되지 않았다면 공격 처리하지 않음
        if (playerController == null || !playerController.hasMace) return;

        HandleAttackInput(); // 공격 입력 처리

        // 콤보 타이머 관리
        if (canCombo)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                canCombo = false; // 콤보 시간 초과 시 콤보 불가
        }
    }

    // 공격 입력 처리 함수
    private void HandleAttackInput()
    {
        // 공격 중이거나 최대 콤보 단계에 도달했으면 입력 무시
        if (attackLocked || currentAttack >= 2) return;

        // J 키 입력으로 공격 시작
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!isAttacking && currentAttack == 0)
            {
                // 첫 번째 공격 시작
                currentAttack = 1;
                StartCoroutine(Attack1());
            }
            else if (canCombo && currentAttack == 1)
            {
                // 두 번째 콤보 공격 시작
                canCombo = false;
                StopAllCoroutines();       // Attack1 중단
                currentAttack = 2;
                StartCoroutine(Attack2()); // Attack2 실행
            }
        }
    }

    // 첫 번째 공격 (Attack1) 처리
    public IEnumerator Attack1()
    {
        isAttacking = true;
        playerController.isAttacking = true; // 이동 제한 설정

        // 공격 중 x축 이동 방지
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Attack1 애니메이션 실행
        animator.SetTrigger("MaceAttack1");

        yield return new WaitForSeconds(0.01f); // 히트박스 타이밍 보정

        // 콤보 입력 가능 상태로 전환
        canCombo = true;
        comboTimer = comboDuration;

        yield return new WaitForSeconds(0.5f); // 애니메이션 지속 시간 대기

        // 공격 종료
        isAttacking = false;
        playerController.isAttacking = false;
        canCombo = false;

        // 콤보로 넘어가지 않으면 초기화
        if (currentAttack == 1) currentAttack = 0;
    }

    // 두 번째 공격 (Attack2) 처리
    private IEnumerator Attack2()
    {
        isAttacking = true;
        playerController.isAttacking = true;

        // 이동 제한
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Attack2 애니메이션 실행
        animator.SetTrigger("MaceAttack2");

        yield return new WaitForSeconds(0.48f); // 애니메이션 동작 시간 대기

        // 콤보 종료
        canCombo = false;
        comboTimer = 0f;

        yield return new WaitForSeconds(0.48f); // 후딜레이 대기

        // 공격 종료
        isAttacking = false;
        playerController.isAttacking = false;
        currentAttack = 0;

        // 입력 잠금 후 짧게 해제
        attackLocked = true;
        yield return new WaitForSeconds(0.01f);
        attackLocked = false;
    }

    // Attack1 사운드 재생 (애니메이션 이벤트로 호출)
    public void PlayMaceAttack1Sound()
    {
        if (Maceattack1Sound != null)
        {
            audioSource.PlayOneShot(Maceattack1Sound);
        }
    }

    // Attack2 사운드 재생 (애니메이션 이벤트로 호출)
    public void PlayMaceAttack2Sound()
    {
        if (Maceattack2Sound != null)
        {
            audioSource.PlayOneShot(Maceattack2Sound);
        }
    }

    // 현재 공격 단계를 반환하는 메서드
    public int GetCurrentAttack()
    {
        return currentAttack;
    }
}