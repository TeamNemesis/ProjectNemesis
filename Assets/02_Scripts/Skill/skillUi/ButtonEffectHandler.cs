using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class CyberpunkButtonEffect : MonoBehaviour
{
		private Button button;
		private UnityEngine.Events.UnityEvent originalOnClick;
		private Image buttonImage;
		private Color originalColor;

		void Start()
		{
				button = GetComponent<Button>();
				buttonImage = GetComponent<Image>();
				if (buttonImage != null) originalColor = buttonImage.color;

				// 기존 OnClick 이벤트 저장
				originalOnClick = button.onClick;

				// 버튼의 기본 onClick은 제거하고, 래퍼만 등록
				button.onClick = new Button.ButtonClickedEvent();
				button.onClick.AddListener(PlayEffect);
		}

		void PlayEffect()
		{
				// DOTween Sequence로 네온 펄스 + 글리치 흔들림 조합
				Sequence seq = DOTween.Sequence();

				// 네온 펄스 (색상 번쩍임)
				if (buttonImage != null)
				{
						seq.Append(buttonImage.DOColor(Color.cyan, 0.1f));
						seq.Append(buttonImage.DOColor(originalColor, 0.1f));
				}

				// 글리치 흔들림 (빠른 좌우 흔들림)
				seq.Join(button.transform.DOShakePosition(
						0.25f,                          // 지속 시간
						strength: new Vector3(10, 0, 0), // 흔들림 강도 (좌우)
						vibrato: 30,                     // 진동 횟수
						randomness: 90,                  // 랜덤성
						snapping: false,                 // 스냅 여부
						fadeOut: true                    // 점점 줄어듦
				));

				// 애니메이션 끝난 뒤 원래 이벤트 실행
				seq.OnComplete(() =>
				{
						originalOnClick?.Invoke();
				});
		}
}
