using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    private enum CoinState { Ejected, Waiting, Seeking }
    private CoinState currentState = CoinState.Ejected;

    public float moveSpeed = 15f;
    public int goldAmount = 0;
    public float seekDelay = 0.5f; // 땅에 떨어진 후 플레이어를 향해 날아갈 때까지의 딜레이

    private Transform target;
    private PlayerStats playerStats;
    private Rigidbody2D rb;
    private CircleCollider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            playerStats = player.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogError("Coin: Player not found! Destroying coin.");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (currentState == CoinState.Seeking && target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                Collect();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == CoinState.Ejected && collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(StartSeekingRoutine());
        }
    }

    private IEnumerator StartSeekingRoutine()
    {
        currentState = CoinState.Waiting;
        
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        col.isTrigger = true;

        yield return new WaitForSeconds(seekDelay);

        currentState = CoinState.Seeking;
    }

    void Collect()
    {
        if (playerStats != null)
        {
            playerStats.AddMoney(goldAmount);
        }
        Destroy(gameObject);
    }
}
