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
    [SerializeField] private GameObject range;
    [SerializeField] private GameObject bladeWave;
    private bool isPattern1 = false;
    Transform player;

    private void Update()
    {
        

        if (isDead || player == null) return;
        if (isStunned) return;

        if (CanSeePlayer())
        {
            LookAtPlayer();
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
                    StartCoroutine(PerformAttack());
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
        //pattern1
        if(!isPattern1)
        {
            Pattern1();

            isPattern1 = true;
        }
        
        



        //agent.SetDestination(player.position);  //기존이동

        if (distance <= attackRange && CanSeePlayer())
        {
            agent.ResetPath();
            currentState = State.Attack;
        }
    }

    private IEnumerator PerformAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        _isAttacking = true;

        if (player != null && distance <= attackRange)  //공격범위 내에 플레이어가 있을 때
        {
            // 몬스터 기준 중심 위치 설정
            Vector3 center = transform.position + transform.forward * (_box_Length / 2f);

            // 박스의 반 크기
            Vector3 halfExtents = new Vector3(_box_Width / 2f, _box_Height / 2f, _box_Length / 2f);

            // 박스의 회전 (몬스터 정면을 기준으로 정렬)
            Quaternion orientation = Quaternion.LookRotation(transform.forward);

            // 박스 영역 안의 적 탐색
            Collider[] hitTarget = Physics.OverlapBox(center, halfExtents, orientation);


            foreach (Collider target in hitTarget)
            {
                if (target.TryGetComponent(out IDamageable playerHealth) && target.tag == targetTag)
                {
                    yield return new WaitForSeconds(0.5f);
                    float finalPlayerDistance = Vector3.Distance(transform.position, player.position);
                    if (finalPlayerDistance <= attackRange)
                    {
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(attackDamage);  //데미지 적용
                        }
                    }
                }
            }
            yield return new WaitForSeconds(attackDelay);   //딜레이
        }
        _isAttacking = false;
        currentState = State.Move; // 공격 후 다시 추격 상태로 전환
    }


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
        //_meshRenderer = GetComponent<MeshRenderer>();   //안보이게
        //_meshRenderer.enabled = false;

        //StartCoroutine(ScaleTime(new Vector3(5, 5, 5), 2f));    //범위 조절

        ////range.transform.localScale = Vector3.one;   //범위 초기화
        //transform.position = new Vector3(player.transform.position.x, 0.1f, player.transform.position.z); //플레이어 위치로 순간이동
        //_meshRenderer.enabled = true;    //보이게
        StartCoroutine(Pattern1Routine());
    }

    private IEnumerator Pattern1Routine()
    {
        //사라지기
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;

        //
        yield return StartCoroutine(ScaleTime());

        //보이기
        _meshRenderer.enabled = true;

        //블레이드 파동 3번
        transform.LookAt(player);

        yield return StartCoroutine(BladeWave());
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


        GameObject rangePrefab = Instantiate(range, spawnPos, range.transform.rotation);  //생성
        yield return new WaitForSeconds(1f);
        transform.position = new Vector3(spawnPos.x, 2f, spawnPos.z);   //range로 위치이동
        Destroy(rangePrefab);

    }
    private IEnumerator BladeWave()
    {
        int waveCount = 3;
        float cool = 1f;

        for (int i = 0; i < waveCount; i++)
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
        GameObject blade = Instantiate(bladeWave, spawnPos, transform.rotation);
    }


    void Pattern2()
    {

    }
}
