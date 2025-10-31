using System.Collections;
using UnityEngine;

public class ShotgunDecalEffect : PoolableObject
{
    [Header("Components")]
    public GameObject baseShape;     // 부모 - 부채꼴 텍스처 (마스크 역할)
    public GameObject fillShape;     // 자식 - 붉은색 Quad (차오르는 부분)

    private Coroutine growRoutine;  // 코루틴 중복 방지용
    private MeshRenderer fillRenderer; // 색상 변경용

    [Header("Colors")]
    public Color startColor = new Color(1f, 0f, 0f, 0.3f); // 연한 빨강
    public Color endColor = new Color(1f, 0f, 0f, 0.6f);   // 진한 빨강

    public void Initialize()
    {
        if (baseShape == null)
        {
            // 자식에서 BaseShape 자동 탐색
            Transform baseChild = transform.Find("BaseShape");
            if (baseChild != null)
            {
                baseShape = baseChild.gameObject;

                // FillShape는 BaseShape의 자식
                Transform fillChild = baseChild.Find("FillShape");
                if (fillChild != null)
                {
                    fillShape = fillChild.gameObject;
                    fillRenderer = fillShape.GetComponent<MeshRenderer>();
                }
            }
        }
    }

    /// <summary>
    /// 외부에서 호출: 부채꼴을 duration 동안 0 → 1 로 차오르게 하고, 색상도 변경
    /// </summary>
    /// <param name="duration">차오르는 시간</param>
    /// <param name="range">부채꼴 최대 거리 (스케일 조정용)</param>
    /// <param name="onComplete">완료 시 콜백 (데미지 판정 등)</param>
    public void Play(float duration, float range, System.Action onComplete = null)
    {
        Initialize();
        if (baseShape == null || fillShape == null) return;

        // 전체 스케일 설정 (범위 조정)
        gameObject.transform.localScale = Vector3.one * range;

        StopAllCoroutines();

        // 초기 상태 설정
        fillShape.transform.localScale = Vector3.zero;

        if (fillRenderer != null)
        {
            fillRenderer.material.color = startColor;
        }

        baseShape.SetActive(true);
        fillShape.SetActive(true);

        growRoutine = StartCoroutine(GrowAndRelease(duration, onComplete));
    }

    private IEnumerator GrowAndRelease(float duration, System.Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // FillShape 스케일 증가 (0 → 1)
            fillShape.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            // 색상 변경 (연한 빨강 → 진한 빨강)
            if (fillRenderer != null)
            {
                Color currentColor = Color.Lerp(startColor, endColor, t);
                fillRenderer.material.color = currentColor;
            }

            yield return null;
        }

        // 최종 상태
        fillShape.transform.localScale = Vector3.one;
        if (fillRenderer != null)
        {
            fillRenderer.material.color = endColor;
        }

        // 잠깐 대기 (플레이어가 최종 상태를 볼 수 있도록)
        yield return new WaitForSeconds(0.1f);

        // 공격 판정 콜백 실행
        onComplete?.Invoke();

        yield return new WaitForSeconds(0.3f);

        // 프리팹 반환
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
    }

    /// <summary>
    /// 강제 종료 및 풀 반환
    /// </summary>
    public void ForceStop()
    {
        StopAllCoroutines();
        if (baseShape != null)
        {
            baseShape.SetActive(false);
        }
        if (fillShape != null)
        {
            fillShape.SetActive(false);
        }

        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
    }
}