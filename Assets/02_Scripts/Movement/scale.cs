using UnityEngine;
using System.Collections;

public class Scale : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ScaleTime(new Vector3(5, 5, 5), 2f));
    }

    private IEnumerator ScaleTime(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

    }
    
}
