using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TouchSpawnDrag : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject prefab; // 생성할 오브젝트
    [SerializeField] private string targetImageName = "TargetImage"; // 터치해야 하는 이미지 이름

    private GameObject spawnedObject;
    private Vector3 spawnOffset = new Vector3(0, 0, 0);
    private bool isDragging = false;

    public InputAction touchAction;

    private void OnEnable()
    {
        touchAction.Enable();
        touchAction.started += OnTouchStarted;
        touchAction.performed += OnTouchMoved;
        touchAction.canceled += OnTouchEnded;
    }

    private void OnDisable()
    {
        touchAction.started -= OnTouchStarted;
        touchAction.performed -= OnTouchMoved;
        touchAction.canceled -= OnTouchEnded;
        touchAction.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = Pointer.current.position.ReadValue();

        // UI 클릭 확인 (EventSystem이 필요)
        if (EventSystem.current.IsPointerOverGameObject())
        {
            var pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = screenPos;
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var hit in results)
            {
                if (hit.gameObject.name == targetImageName)
                {
                    // 터치한 UI 이름이 특정 이미지일 경우
                    Vector3 worldPos = ScreenToWorld(screenPos);
                    spawnedObject = Instantiate(prefab, worldPos, Quaternion.identity);
                    isDragging = true;
                    return;
                }
            }
        }
    }

    private void OnTouchMoved(InputAction.CallbackContext ctx)
    {
        if (!isDragging || spawnedObject == null) return;

        Vector2 screenPos = Pointer.current.position.ReadValue();
        Vector3 worldPos = ScreenToWorld(screenPos);
        spawnedObject.transform.position = worldPos;
    }

    private void OnTouchEnded(InputAction.CallbackContext ctx)
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        isDragging = false;
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        // y = 0 평면 기준으로 변환 (3D용)
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float distance;
        if (ground.Raycast(ray, out distance))
            return ray.GetPoint(distance);
        return Vector3.zero;
    }
}
