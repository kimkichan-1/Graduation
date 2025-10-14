using UnityEngine;

public class Pearl : MonoBehaviour
{
    [Header("효과 (선택사항)")]
    [SerializeField] private GameObject collectionEffectPrefab; // 획득 시 나타날 이펙트 (파티클 등)
    [SerializeField] private AudioClip collectionSound;       // 획득 시 재생될 사운드

    // isTrigger가 켜진 다른 Collider와 부딪혔을 때 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 부딪힌 대상이 "Player" 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        // 1. PearlDisplayUI에 진주를 추가하라고 알립니다.
        if (PearlDisplayUI.instance != null)
        {
            PearlDisplayUI.instance.AddPearl(1);
        }

        // 2. 획득 이펙트가 있다면 생성합니다.
        if (collectionEffectPrefab != null)
        {
            Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 3. 획득 사운드가 있다면 재생합니다. (AudioSource가 있는 오브젝트에서 재생)
        if (collectionSound != null)
        {
            // 간단한 방법: 월드 위치에 오디오를 재생하고 잠시 후 삭제
            AudioSource.PlayClipAtPoint(collectionSound, transform.position);
        }

        // 4. 진주 오브젝트 자신을 파괴하여 사라지게 합니다.
        Destroy(gameObject);
    }
}
