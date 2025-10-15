using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class DragInputManager : MonoBehaviour
{
    //private bool isDragging = false;
    //private bool isLeftSide = false;

    //[SerializeField] private GameObject leftStick;
    //[SerializeField] private GameObject rightStick;
    

    //[SerializeField] private Canvas canvas;

    [SerializeField] private GameObject rangePrefab;
    [SerializeField] private Camera mainCamera;
    private GameObject currentRange;
    private bool isDragging = false;
    [SerializeField] private LayerMask groundMask;

//    void Update()
//    {
//#if UNITY_EDITOR
//        // 마우스 테스트용 (에디터)
//        if (Input.GetMouseButtonDown(0))
//        {
//            StartDrag(Input.mousePosition);
//        }
//        else if (Input.GetMouseButton(0) && isDragging)
//        {
//            ContinueDrag(Input.mousePosition);
//        }
//        else if (Input.GetMouseButtonUp(0) && isDragging)
//        {
//            EndDrag();
//        }
//#else
//        // 모바일 터치용
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);

//            switch (touch.phase)
//            {
//                case TouchPhase.Began:
//                    StartDrag(touch.position);
//                    break;
//                case TouchPhase.Moved:
//                case TouchPhase.Stationary:
//                    if (isDragging) ContinueDrag(touch.position);
//                    break;
//                case TouchPhase.Ended:
//                case TouchPhase.Canceled:
//                    if (isDragging) EndDrag();
//                    break;
//            }
//        }
//#endif
//    }

    //void StartDrag(Vector2 position)
    //{
    //    isDragging = true;
    //    isLeftSide = position.x < Screen.width / 2f;
    //    MoveStickToPosition(position);

    //    //if (isLeftSide)
    //    //    Debug.Log("왼쪽드래그 시작");
    //    //else
    //    //    Debug.Log("오른쪽드래그 시작");
    //    if (isLeftSide)
    //    {
    //        //Debug.Log("왼쪽드래그 중");

    //        leftStick.SetActive(true);
    //    }
    //    else
    //    {
    //        //Debug.Log("오른쪽드래그 중");
    //        rightStick.SetActive(true);
    //    }

    //}

    //void ContinueDrag(Vector2 position)
    //{
    //    if (isLeftSide)
    //    {
    //        Debug.Log("왼쪽드래그 중");

    //    }
    //    else
    //    {
    //        Debug.Log("오른쪽드래그 중");

    //    }
            
    //}
    
    //void EndDrag()
    //{
    //    if (isLeftSide)
    //    {
    //        //Debug.Log("왼쪽드롭");
    //        leftStick.SetActive(false);
    //    }
    //    else
    //    {
    //        //Debug.Log("오른쪽드롭");
    //        rightStick.SetActive(false);
    //    }

    //    isDragging = false;
    //}
    //void MoveStickToPosition(Vector2 screenPos)
    //{
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        canvas.transform as RectTransform,
    //        screenPos,
    //        canvas.worldCamera,
    //        out Vector2 localPoint
    //    );

    //    if (isLeftSide)
    //    {
    //        leftStick.GetComponent<RectTransform>().anchoredPosition = localPoint;
    //    }
    //    else
    //    {
    //        rightStick.GetComponent<RectTransform>().anchoredPosition = localPoint;
    //    }
    //}

    public void StartGrenade()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()),
                                out RaycastHit hit,
                                100f,
                                groundMask))
        {
            currentRange = Instantiate(rangePrefab, hit.point, Quaternion.identity);
            isDragging = true;
        }
    }
    public void DragGrenade()
    {
        if (isDragging && currentRange != null)
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()),
                                out RaycastHit hit,
                                100f,
                                groundMask))
            {
                Vector3 pos = hit.point;
                pos.y = 0; // 필요 시 고정
                currentRange.transform.position = pos;
            }
        }
    }
    public void EndGrenade()
    {
        if (currentRange != null)
            Destroy(currentRange);

        isDragging = false;
    }
}
