using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damaged : MonoBehaviour
{
    [SerializeField] private Image panelImage;

    private void Update()
    {
        // 숫자키 0 = KeyCode.Alpha0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(FadePanel());
        }
    }

    private IEnumerator FadePanel()
    {
        // 1) 알파 0.3으로 설정
        Color c = panelImage.color;
        c.a = 0.3f;
        panelImage.color = c;

        // 2) 1초 동안 알파를 0까지 줄이기
        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0.3f, 0f, time / duration);

            c.a = alpha;
            panelImage.color = c;

            yield return null;
        }

        // 마지막에 확실하게 알파 0으로
        c.a = 0f;
        panelImage.color = c;
    }
}
