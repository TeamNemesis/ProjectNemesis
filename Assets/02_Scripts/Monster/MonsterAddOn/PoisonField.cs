using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using static DebuffHandler;

public class PoisonField : PoolableObject
{
    // trigger로 작동하며 플레이어가 접촉시 즉시 발동 (코루틴 적용)
    // 현재는 Instantiate 기준으로 만듦, 오브젝트 풀링으로 변경 시 수정 필요

    [SerializeField] private float tickDuration = 1f; // 피해 주기
    [SerializeField] private Coroutine debuffCoroutine; // 디버프 코루틴 참조
    [SerializeField] private float lifeTime = 999f; // 독성 장판 지속 시간
    [SerializeField] private float poisonFieldRadius;
    [SerializeField] private string targetTag; // 타겟 태그
    [SerializeField] private Coroutine lifeTimeCoroutine;
    [SerializeField] private bool isPoison = true;

    public void Initialize(string targetTag, float lifeTime, float poisonFieldRadius)
    {
        SetTarget(targetTag);
        SetLifeTime(lifeTime);
        SetRadius(poisonFieldRadius);
        SetScale();
        StartLifeTime();
    }

    public void SetTarget(string targetTag)
    {
        this.targetTag = targetTag;
    }
    public void SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
    }
    public void SetRadius(float poisonFieldRadius)
    {
        this.poisonFieldRadius = poisonFieldRadius;
    }

    private void StartLifeTime()
    {
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
        }

        lifeTimeCoroutine = StartCoroutine(LifeTimeCoroutine());
    }

    // 플레이어가 트리거에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && isPoison)
        {
            DebuffHandler debuffHandler = other.GetComponent<DebuffHandler>();
            if (debuffHandler != null && debuffCoroutine == null)
            {
                // 참조 코루틴 생성
                debuffCoroutine = StartCoroutine(DebuffToTarget(debuffHandler));
            }
        }
    }

    // 플레이어가 트리거에서 나갔을 때
    private void OnTriggerExit(Collider other)
    { 
        {
            if (debuffCoroutine != null)
            {
                // 코루틴 중지
                StopCoroutine(debuffCoroutine);
                // 참조 코루틴 삭제
                debuffCoroutine = null;
            }
        }
    }

    private IEnumerator DebuffToTarget(DebuffHandler debuffHandler)
    {
        while (true)
        {
            DebuffData poison = DebuffData.CreatePoison();
            debuffHandler.ApplyDebuff(poison);
            yield return new WaitForSeconds(tickDuration);
        }
    }

    private IEnumerator LifeTimeCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
    }

    private void SetScale ()
    {
        gameObject.transform.localScale = new Vector3(poisonFieldRadius, 1, poisonFieldRadius);
    }
}