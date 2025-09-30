using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hAxis;
    public float vAxis;
    public float speed;

    public float clickMoveDistance = 2f; //

    Vector3 moveVec;
    void Start()
    {


    }
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); //
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;

        transform.LookAt(transform.position + moveVec);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Vector3 targetPos = hit.point;
                targetPos.y = transform.position.y; // 캐릭터 높이 유지
                transform.LookAt(targetPos);

                transform.position += transform.forward * clickMoveDistance; // 바라본 방향으로 앞으로 조금 이동
            }
        }
    }
}
