using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float cameraSpeed;

    [Header("카메라 제한 영역")]
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    void Start()
    {

    }
    void Update()
    {
        //transform.position = target.position + offset;

        // 기본 카메라 위치
        Vector3 desiredPos = target.position + offset;

        // X, Z 좌표를 제한 (카메라가 맵 밖을 보지 않게)
        float clampX = Mathf.Clamp(desiredPos.x, minX, maxX);
        float clampZ = Mathf.Clamp(desiredPos.z, minZ, maxZ);

        //transform.position = new Vector3(clampX, desiredPos.y, clampZ);
        transform.position = Vector3.Lerp(transform.position, new Vector3(clampX, desiredPos.y, clampZ), cameraSpeed * Time.deltaTime);//Lerp사용
    }
}
