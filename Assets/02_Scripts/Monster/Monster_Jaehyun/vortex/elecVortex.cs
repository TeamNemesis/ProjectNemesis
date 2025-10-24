using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.AI;

public class elecVortex : MonoBehaviour
{
    [SerializeField] private GameObject[] monsters;     // 임시 설정
    [SerializeField] private float speed;               // 따라갈 속도

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
        Vector3 pos1 = new Vector3(Tr.position.x, Tr.position.y - ConstHeight, Tr.position.z);  //캡슐의 맨아래 위치
        Vector3 pos2 = new Vector3(Tr.position.x, Tr.position.y + ConstHeight, Tr.position.z);  //캡슐의 맨위 위치
        colliders = Physics.OverlapCapsule(pos2, pos1, radius, layer);  //실제 범위

        foreach (var col in colliders)
        {
            NavMeshAgent agent = col.GetComponent<NavMeshAgent>();

            // Point자식
            Transform point = transform.Find("Point");

            // 이동 방향 계산 (Point의 위치 - 현재 위치)
            Vector3 dir = (point.position - col.transform.position).normalized;

            // 힘 적용
            agent.Move(dir * Time.deltaTime * power);
        }

        //배열에서 가까운오브젝트 찾고 이동. 
        GameObject nearest = GetNearestMonster();
        if (nearest != null)
        {
            Vector3 targetPos = nearest.transform.position;
            Vector3 moveDir = (targetPos - transform.position).normalized;

            // 🔹 3. speed 속도로 이동
            transform.position += moveDir * speed * Time.deltaTime;
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
