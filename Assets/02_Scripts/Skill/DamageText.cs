using TMPro;
using UnityEngine;

public class DamageText : PoolableObject
{
		public TMP_Text text;
		public float floatSpeed = 1f;
		public float duration = 0.5f;
		public float fadeDuration = 0.3f;

		private float timer = 0f;
		private Color originalColor;

		void OnEnable()
		{
				timer = 0f;
				originalColor = text.color;
		}

		public void SetDmg(int damage)
		{
				text.text = damage.ToString();
				StartCoroutine(FloatAndFade());
		}

		private System.Collections.IEnumerator FloatAndFade()
		{
				Vector3 startPos = transform.position;
				Vector3 endPos = startPos + Vector3.up * 1f;

				while (timer < duration)
				{
						timer += Time.deltaTime;
						float t = timer / duration;

						// 위치 이동
						transform.position = Vector3.Lerp(startPos, endPos, t);

						// 알파 페이드
						if (timer > duration - fadeDuration)
						{
								float fadeT = (timer - (duration - fadeDuration)) / fadeDuration;
								Color c = originalColor;
								c.a = Mathf.Lerp(1f, 0f, fadeT);
								text.color = c;
						}

						yield return null;
				}

				// 풀로 반환
				GameManager.Instance.PoolManager.ReleaseToPool(this.gameObject);
		}
}
