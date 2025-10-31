using System.Collections;
using UnityEngine;

public class ShotgunDecalEffect : PoolableObject
{
    private MeshRenderer meshRenderer;
    private Material materialInstance;
    private Coroutine fadeRoutine;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    public float startAlpha = 0.3f;
    public float endAlpha = 1.0f;

    public void Play()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeAndReturn());
    }

    private IEnumerator FadeAndReturn()
    {

        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (meshRenderer != null)
        {
            materialInstance = meshRenderer.material;
        }
        if (materialInstance == null) yield break;

        float elapsed = 0f;
        Color color = materialInstance.color;

        // 알파값을 startAlpha로 시작
        color.a = startAlpha;
        materialInstance.color = color;

        // fadeDuration 동안 알파값을 endAlpha까지 증가
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            materialInstance.color = color;

            yield return null;
        }

        // 최종 알파값 설정
        color.a = endAlpha;
        materialInstance.color = color;

        // 풀로 반환
        GameManager.Instance.PoolManager.ReleaseToPool(gameObject);
    }
}