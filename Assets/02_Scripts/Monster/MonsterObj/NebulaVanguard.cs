using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class NebulaVanguard : MonsterBase
{
    [SerializeField]
    private enum State
    {
        Idle,   // 플레이어를 아직 못 찾았거나 감지 범위 밖일 때
        Move,   // 플레이어를 추격 중일 때
        Attack, // 공격
        Die     // 죽음
    }
    [Header("Local Stats")]
    [SerializeField] private bool _isAttacking = false;
    [SerializeField] private float _box_Length = 3;
    [SerializeField] private float _box_Height = 3;
    [SerializeField] private float _box_Width = 3;

    [SerializeField]
    private State currentState = State.Idle;

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private GameObject rangePrefab;
    [SerializeField] private GameObject bladeWavePrefab;
    [SerializeField] private GameObject BulletPrefab;
    private bool isPattern = false;
    private int attackCount = 2;
    private float attackDist = 1f; // 공격 시 전진 거리

    private void Update()
    {
        

        if (isDead || player == null) return;
        if (isStunned) return;

        if (CanSeePlayer())
        {
            LookAtPlayer();
        }
        if(currentHealth <= 50f)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Move:
                HandleMove();
                break;
            case State.Attack:
                if (!_isAttacking)
                {
                    //StartCoroutine(PerformAttackOnce());
                }
                break;
            case State.Die:
                Die();
                break;
        }
    }


    private void HandleIdle()
    {
        // 플레이어와 거리
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange && CanSeePlayer())
        {
            currentState = State.Move;
        }
    }
    private void HandleMove()
    {
        if (player == null) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange || !CanSeePlayer())   //플레이어와의 거리 > 탐지거리 ->이때 패턴 시작
        {
            agent.ResetPath();
            currentState = State.Idle;
            return;
        }

        if (!_isAttacking)//패턴 하나 끝나면
        {
            //랜덤 패턴 선택
            int randomPattern = Random.Range(1, 3);

            if (randomPattern == 1)
            {
                Debug.Log("패턴1 시작");
                Pattern1();
            }
            else
            {
                Debug.Log("패턴2 시작");
                Pattern2();
            }     
        }
    }

    //private IEnumerator PerformAttack()
    //{
    //    float distance = Vector3.Distance(transform.position, player.position);
    //    _isAttacking = true;

    //    if (player != null && distance <= attackRange)  //공격범위 내에 플레이어가 있을 때
    //    {
    //        // 몬스터 기준 중심 위치 설정
    //        Vector3 center = transform.position + transform.forward * (_box_Length / 2f);

    //        // 박스의 반 크기
    //        Vector3 halfExtents = new Vector3(_box_Width / 2f, _box_Height / 2f, _box_Length / 2f);

    //        // 박스의 회전 (몬스터 정면을 기준으로 정렬)
    //        Quaternion orientation = Quaternion.LookRotation(transform.forward);

    //        // 박스 영역 안의 적 탐색
    //        Collider[] hitTarget = Physics.OverlapBox(center, halfExtents, orientation);


    //        foreach (Collider target in hitTarget)
    //        {
    //            if (target.TryGetComponent(out IDamageable playerHealth) && target.tag == targetTag)
    //            {
    //                yield return new WaitForSeconds(0.5f);
    //                float finalPlayerDistance = Vector3.Distance(transform.position, player.position);
    //                if (finalPlayerDistance <= attackRange)
    //                {
    //                    if (playerHealth != null)
    //                    {

    //                        playerHealth.TakeDamage(attackDamage);  //데미지 적용
                            
    //                    }
    //                }
    //            }
    //        }
    //        yield return new WaitForSeconds(attackDelay);   //딜레이
    //    }
    //    _isAttacking = false;
    //    currentState = State.Move; // 공격 후 다시 추격 상태로 전환
    //}


    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + transform.forward * (_box_Length / 2f);
        Vector3 halfExtents = new Vector3(_box_Width / 2f, _box_Height / 2f, _box_Length / 2f);
        Quaternion orientation = Quaternion.LookRotation(transform.forward);

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, orientation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
    }

    void Pattern1()
    {
        StartCoroutine(Pattern1Routine());
    }

    private IEnumerator Pattern1Routine()
    {
        _isAttacking = true;
        //사라지기
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;

        //
        yield return StartCoroutine(ScaleTime());

        //보이기
        _meshRenderer.enabled = true;

        //블레이드 파동 3번
        transform.LookAt(player);

        if(currentHealth  <= 50)
        {
            yield return StartCoroutine(BladeWave(5, 1f));
        }
        else
        {
            yield return StartCoroutine(BladeWave(3, 1f));
        }
        _isAttacking = false;
    }

   
    private IEnumerator ScaleTime()
    {
        Vector3[] spawnPoints =
        {
            new Vector3( 2f, -0.9f,  0f), // +X 방향
            new Vector3(-2f, -0.9f,  0f), // -X 방향
            new Vector3( 0f, -0.9f,  2f), // +Z 방향
            new Vector3( 0f, -0.9f, -2f)  // -Z 방향
        };

        // 랜덤 인덱스 선택
        int randomIndex = Random.Range(0, spawnPoints.Length);

        // 최종 위치 계산
        Vector3 spawnPos = player.transform.position + spawnPoints[randomIndex];    //플레이어위치+동서남북


        GameObject range = Instantiate(rangePrefab, spawnPos, rangePrefab.transform.rotation);  //생성
        yield return new WaitForSeconds(1f);
        transform.position = new Vector3(spawnPos.x, 2f, spawnPos.z);   //range로 위치이동
        Destroy(range);

    }
    private IEnumerator BladeWave(int count , float cool)
    {
        for (int i = 0; i < count; i++)
        {
            // 프리팹 발사
            ShootBlade();

            // 1초 대기
            yield return new WaitForSeconds(cool);
        }
    }
    void ShootBlade()
    {
        // 플레이어의 위치를 가져오되, 높이는 몬스터 높이와 동일하게 맞추기
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);

        // 수평 방향으로만 LookAt
        transform.LookAt(targetPos);

        // 생성
        Vector3 spawnPos = new Vector3(transform.position.x, 1f, transform.position.z) + transform.forward * 1f;
        GameObject blade = Instantiate(bladeWavePrefab, spawnPos, transform.rotation);
    }

    void Pattern2()
    {
        StartCoroutine(Pattern2Routine());
    }
    private IEnumerator Pattern2Routine()
    {
        _isAttacking = true;
        // 플레이어 방향 계산
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        // 플레이어에서 일정 거리 떨어진
        int stopDistance = 3;
        Vector3 dashTarget = player.position - dirToPlayer * stopDistance;

        // 대쉬 실행
        yield return StartCoroutine(Dash(dashTarget));

        // 짧은 대기
        yield return new WaitForSeconds(0.5f);

        // 공격 2회 반복
        for (int i = 0; i < attackCount; i++)
        {
            yield return StartCoroutine(PerformAttackOnce());
            Debug.Log($"공격 {i + 1}회 완료");
        }

        //yield return new WaitForSeconds(1f);

        if(currentHealth <= 50)
            yield return StartCoroutine(Shoot(5, 0.5f));
        else
        {
            yield return StartCoroutine(Shoot(3, 0.5f));
        }
        //currentState = State.Attack;
        _isAttacking = false;
    }

    private IEnumerator PerformAttackOnce()
    {
        // 공격 시작: 플레이어를 바라보고 앞으로 전진
        transform.LookAt(player);
        transform.position += transform.forward * attackDist;

        // 공격 모션 타이밍 (애니메이션 타이밍 맞추는 용도)
        yield return new WaitForSeconds(0.3f);

        // 데미지 판정 (박스 안에 있을 때만)
        Vector3 center = transform.position + transform.forward * (_box_Length / 2f);
        Vector3 halfExtents = new Vector3(_box_Width / 2f, _box_Height / 2f, _box_Length / 2f);
        Quaternion orientation = Quaternion.LookRotation(transform.forward);

        Collider[] hitTargets = Physics.OverlapBox(center, halfExtents, orientation);

        foreach (Collider target in hitTargets)
        {
            if (target.CompareTag(targetTag) && target.TryGetComponent(out IDamageable playerHealth))
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        // 공격 후 대기 (공격 쿨타임)
        yield return new WaitForSeconds(attackDelay);

        
    }
    private IEnumerator Dash(Vector3 destination)
    {
        agent.isStopped = false;    //활성화
        agent.SetDestination(destination);

        // 경로 계산 중일 때 기다림
        while (agent.pathPending)
            yield return null;
        

        // 도착할 때까지 대기
        while (agent.remainingDistance > agent.stoppingDistance)//플레이어와의 거리가 멈춤거리보다 클 때까지
            yield return null;
        

        // 도착 후 멈춤
        agent.isStopped = true;
        agent.ResetPath();
        yield return null;
    }

    private IEnumerator Shoot(int count, float cool)
    {
        for (int i = 0; i < count; i++)
        {
            // 프리팹 발사
            ShootBullet();

            // 1초 대기
            yield return new WaitForSeconds(cool);
        }
    }
    void ShootBullet()
    {
        // 플레이어의 위치를 가져오되, 높이는 몬스터 높이와 동일하게 맞추기
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);

        // 수평 방향으로만 LookAt
        transform.LookAt(targetPos);

        // 생성
        Vector3 spawnPos = new Vector3(transform.position.x, 1f, transform.position.z) + transform.forward * 1f;
        GameObject bullet = Instantiate(BulletPrefab, spawnPos, transform.rotation);
    }

}
