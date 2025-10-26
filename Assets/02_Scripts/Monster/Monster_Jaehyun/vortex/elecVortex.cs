using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.AI;

public class elecVortex : MonoBehaviour
{
    [SerializeField] private GameObject[] monsters;     // 임시 설정
    [SerializeField] private float speed=1f;               // 따라갈 속도

    [SerializeField] private LayerMask layer;           // 아마 달렸있을 Enemy Layer
    [SerializeField] private Collider[] colliders;      // 감지한 Collder배열(끌어당길)

    [SerializeField] private float radius = 5f;         // 반지름
    [SerializeField] private float height = 2f;         // 범위 높이

    private Transform Tr;
    [SerializeField] private int ConstHeight = 5;
    [SerializeField] private float power = 10f;         // 적용할 힘의 세기

    [SerializeField] private int damage = 20;           // 초당 데미지


    void Start()
    {
        Tr = GetComponent<Transform>();
    }

  
void Update()
    {
        Vector3 pos1 = new Vector3(Tr.position.x, Tr.position.y - ConstHeight, Tr.position.z);
        Vector3 pos2 = new Vector3(Tr.position.x, Tr.position.y + ConstHeight, Tr.position.z);
        colliders = Physics.OverlapCapsule(pos2, pos1, radius, layer);

        Transform point = transform.Find("Point");

        foreach (var col in colliders)
        {
            NavMeshAgent agent = col.GetComponent<NavMeshAgent>();
            if (agent == null) continue;

            Vector3 targetPos = point.position;
            targetPos.y = col.transform.position.y; // 수직 이동 방지

            // 👉 Lerp로 천천히 당기기
            Vector3 newPos = Vector3.Lerp(col.transform.position, targetPos, Time.deltaTime * power);
            agent.Warp(newPos); // Move 대신 Warp로 안정 이동
        }

        // 👉 가장 가까운 몬스터로 Vortex가 부드럽게 이동
        GameObject nearest = GetNearestMonster();
        if (nearest != null)
        {
            Vector3 targetPos = nearest.transform.position;
            targetPos.y = transform.position.y; // Y값 고정

            // Lerp로 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

            // Y값 완전히 고정 (원하면)
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }
    private GameObject GetNearestMonster()
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (var monster in monsters)
        {
            if (monster == null) continue;

            float dist = Vector3.Distance(transform.position, monster.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = monster;
            }
        }

        return nearest;
    }

    void OnDrawGizmosSelected() //실린더 모양 기즈모
    {
        // 기즈모 색상 설정
        Gizmos.color = Color.red;

        // 기즈모의 변환 행렬을 현재 오브젝트에 맞게 설정
        // 이렇게 하면 기즈모가 오브젝트의 위치, 회전, 스케일을 따라갑니다.
        Gizmos.matrix = transform.localToWorldMatrix;

        // 원통의 상단과 하단 원 그리기
        Vector3 top = Vector3.up * height * 0.5f;
        Vector3 bottom = Vector3.down * height * 0.5f;
        Gizmos.DrawWireSphere(top, radius);
        Gizmos.DrawWireSphere(bottom, radius);

        // 상단과 하단 원을 잇는 4개의 선 그리기
        Vector3 forward = Vector3.forward * radius;
        Vector3 back = Vector3.back * radius;
        Vector3 right = Vector3.right * radius;
        Vector3 left = Vector3.left * radius;

        Gizmos.DrawLine(top + forward, bottom + forward);
        Gizmos.DrawLine(top + back, bottom + back);
        Gizmos.DrawLine(top + right, bottom + right);
        Gizmos.DrawLine(top + left, bottom + left);
    }
}
