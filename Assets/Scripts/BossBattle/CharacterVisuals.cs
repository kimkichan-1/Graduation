using System.Collections;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    public bool facesRightByDefault = true;
    [Header("위치 설정")]
    public Transform homeTransform;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public IEnumerator Knockback(float distance, float duration)
    {
        Vector3 direction = (homeTransform.position - transform.position).normalized;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction * distance;

        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    public IEnumerator ReturnToHomePosition(float duration)
    {
        if (homeTransform != null)
        {
            yield return StartCoroutine(MoveToPosition(homeTransform.position, duration));
        }
        else
        {
            Debug.LogWarning(gameObject.name + "에게 homeTransform이 지정되지 않았습니다!");
        }
    }

    public void FaceOpponent(Transform opponentTransform)
    {
        if (spriteRenderer == null) return;
        float xDirection = opponentTransform.position.x - transform.position.x;
        float localScaleX = transform.localScale.x;

        if (xDirection > 0)
        {
            if (facesRightByDefault)
                localScaleX = Mathf.Abs(transform.localScale.x);
            else
                localScaleX = -Mathf.Abs(transform.localScale.x);
        }
        else if (xDirection < 0)
        {
            if (facesRightByDefault)
                localScaleX = -Mathf.Abs(transform.localScale.x);
            else
                localScaleX = Mathf.Abs(transform.localScale.x);
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }
}