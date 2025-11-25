using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ButtonEffectHandler : MonoBehaviour
{
		private Button button;
		private UnityEngine.Events.UnityEvent originalOnClick;

		void Awake()
		{
				button = GetComponent<Button>();

				// 기존 OnClick 이벤트 저장
				originalOnClick = button.onClick;

				// 버튼의 기본 onClick은 제거하고, 래퍼만 등록
				button.onClick = new Button.ButtonClickedEvent();
				button.onClick.AddListener(PlayEffect);
		}

		void PlayEffect()
		{
				// 애니메이션 실행
				transform.DOScale(1.2f, 0.1f)
						.SetLoops(2, LoopType.Yoyo)
						.OnComplete(() =>
						{
								// 애니메이션 끝난 뒤 기존 이벤트 실행
								originalOnClick?.Invoke();
						});
		}
}
