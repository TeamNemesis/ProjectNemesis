using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class test : MonoBehaviour
{
    [Header("Sticks")]
    [SerializeField] private OnScreenStick leftStick;
    [SerializeField] private OnScreenStick rightStick;

    [Header("UI Groups (for visibility control)")]
    [SerializeField] private CanvasGroup leftGroup;
    [SerializeField] private CanvasGroup rightGroup;

    [Header("Canvas")]
    [SerializeField] private Canvas canvas;

    private bool isDragging = false;
    private bool isLeftSide = false;
    private OnScreenStick activeStick;
    private CanvasGroup activeGroup;

    void Start()
    {
        // 처음엔 투명하게
        ShowStick(leftGroup, false);
        ShowStick(rightGroup, false);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            StartDrag(Input.mousePosition);
        else if (Input.GetMouseButton(0) && isDragging)
            ContinueDrag(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0) && isDragging)
            EndDrag();
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartDrag(touch.position);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isDragging) ContinueDrag(touch.position);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isDragging) EndDrag();
                    break;
            }
        }
#endif
    }

    void StartDrag(Vector2 position)
    {
        isDragging = true;
        isLeftSide = position.x < Screen.width / 2f;
        activeStick = isLeftSide ? leftStick : rightStick;
        activeGroup = isLeftSide ? leftGroup : rightGroup;

        MoveStickBaseTo(position);
        ShowStick(activeGroup, true);
    }

    void ContinueDrag(Vector2 position)
    {
        // On-Screen Stick이 자동으로 InputAction 값을 처리하므로
        // UI상 핸들은 OnScreenStick 자체에서 움직임
        // 별도 처리 불필요 (단, 베이스 고정 위치로 유지 가능)
    }

    void EndDrag()
    {
        ShowStick(activeGroup, false);
        isDragging = false;
        activeStick = null;
        activeGroup = null;
    }

    void MoveStickBaseTo(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        if (isLeftSide)
            leftStick.GetComponent<RectTransform>().anchoredPosition = localPoint;
        else
            rightStick.GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    void ShowStick(CanvasGroup g, bool show)
    {
        g.alpha = show ? 1 : 0;
        g.blocksRaycasts = show;  // 터치 이벤트 활성화
    }
}
