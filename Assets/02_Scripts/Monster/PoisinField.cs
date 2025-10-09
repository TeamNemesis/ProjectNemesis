using System.Collections;
using UnityEngine;

public class PoisinField : MonoBehaviour
{
    // trigger로 작동하며 플레이어가 접촉시 즉시 발동 (코루틴 적용)
    // 현재는 Instantiate 기준으로 만듦, 오브젝트 풀링으로 변경 시 수정 필요

    [SerializeField] private float tickDuration = 1f; // 피해 주기
    [SerializeField] private float moveSpeed = 3f; // 이동 속도
    [SerializeField] private Coroutine damageCoroutine; // 피해 코루틴 참조


    private void Start()
    {
        StartCoroutine(MoveFront());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && damageCoroutine == null)
            {
                // 참조 코루틴 생성
                damageCoroutine = StartCoroutine(DamageToPlayer(playerHealth));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                // 코루틴 중지
                StopCoroutine(damageCoroutine);
                // 참조 코루틴 삭제
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DamageToPlayer(PlayerHealth playerHealth)
    {
        while (true)
        {
            // 1초마다 3의 피해
            playerHealth.TakeDamage(3);
            Debug.Log("Player Health: " + playerHealth.currentHealth);
            yield return new WaitForSeconds(tickDuration);
        }
    }

    private IEnumerator MoveFront()
    {
        while (true)
        {
            // 오브젝트의 로컬 앞 방향으로 이동 (Vector3.forward는 로컬 기준)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            // 한 프레임 쉬기
            yield return null;
        }
    }
}