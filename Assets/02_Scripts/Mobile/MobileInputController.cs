using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileInputController : MonoBehaviour
{
		[Header("조이스틱 및 버튼")]
		[SerializeField] private Joystick joystick;
		[SerializeField] private Button dashButton;
		[SerializeField] private Button normalAttackButton;
		[SerializeField] private Button specialAttackButton;
		[SerializeField] private Button grenadeButton;

		[Header("연결 대상")]
		[SerializeField] private Player player;
		[SerializeField] private PlayerInputHandler inputHandler;

		private bool isHoldingNormalAttack = false;
		private bool isInit = false;
		private bool isGrenadeAiming = false;
		private bool isTouchOverButton = false;
		private Vector3 grenadeTarget;
		public void Initialize(Player player,PlayerInputHandler input)
		{
				this.player = player;
				inputHandler = input;
				dashButton.onClick.AddListener(OnDashPressed);
				grenadeButton.onClick.AddListener(OnGrenadePressed);

				AddEventTrigger(normalAttackButton.gameObject, EventTriggerType.PointerDown, OnNormalAttackDown);
				AddEventTrigger(normalAttackButton.gameObject, EventTriggerType.PointerUp, OnNormalAttackUp);
				AddEventTrigger(specialAttackButton.gameObject, EventTriggerType.PointerDown, OnSpecialAttackDown);
				AddEventTrigger(specialAttackButton.gameObject, EventTriggerType.PointerUp, OnSpecialAttackUp);
				AddEventTrigger(grenadeButton.gameObject, EventTriggerType.PointerDown, OnGrenadeDown);
				AddEventTrigger(grenadeButton.gameObject, EventTriggerType.PointerUp, OnGrenadeUp);
				AddEventTrigger(grenadeButton.gameObject, EventTriggerType.PointerEnter, OnGrenadeEnter);
				AddEventTrigger(grenadeButton.gameObject, EventTriggerType.PointerExit, OnGrenadeExit);

				isInit = true;
		}

		private void Update()
		{
				if (!isInit || player == null || inputHandler == null || !EventBus.CanGetInput)
						return;

				Vector2 dir = joystick.Direction;
				Vector3 moveDir = new Vector3(dir.x, 0f, dir.y);

				if (!isHoldingNormalAttack)
				{
						inputHandler.TriggerMoveInput(moveDir);
				}
				else
				{
						inputHandler.TriggerMoveInput(Vector3.zero);
						inputHandler.TriggerNormalAttackInput();
				}

				if (isGrenadeAiming)
				{
						Vector3? target = GetTouchGroundPoint();
						if (target.HasValue)
						{
								grenadeTarget = target.Value;
								player.GrenadeAttacker.UpdateAiming(grenadeTarget);
						}
				}
		}

		private void OnDashPressed()
		{
				inputHandler?.TriggerDashInput();
		}

		private void OnGrenadePressed()
		{
				Vector3? target = GetTouchGroundPoint();
				if (target.HasValue)
				{
						inputHandler?.TriggerGrenadeAttackInput(target.Value);
				}
		}

		private void OnNormalAttackDown(BaseEventData data)
		{
				isHoldingNormalAttack = true;
		}

		private void OnNormalAttackUp(BaseEventData data)
		{
				isHoldingNormalAttack = false;
				
		}

		private void OnSpecialAttackDown(BaseEventData data)
		{
				inputHandler?.TriggerSpecialAttackInput();
		}

		private void OnSpecialAttackUp(BaseEventData data)
		{
				inputHandler?.TriggerSpecialAttackCanceled();
		}

		private Vector3? GetTouchGroundPoint()
		{
				if (Input.touchCount == 0)
						return null;

				Vector2 touchPos = Input.GetTouch(0).position;
				Ray ray = Camera.main.ScreenPointToRay(touchPos);
				if (Physics.Raycast(ray, out RaycastHit hit, 200f, LayerMask.GetMask("Ground")))
				{
						return hit.point;
				}

				return null;
		}

		private void OnGrenadeDown(BaseEventData data)
		{
				isGrenadeAiming = true;
				isTouchOverButton = true;
				player.GrenadeAttacker.StartAiming();
		}

		private void OnGrenadeUp(BaseEventData data)
		{
				if (!isGrenadeAiming) return;

				isGrenadeAiming = false;

				if (isTouchOverButton)
						player.GrenadeAttacker.CancelAiming();
				else
						player.GrenadeAttacker.ConfirmAiming();
		}

		private void AddEventTrigger(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
		{
				EventTrigger trigger = obj.GetComponent<EventTrigger>();
				if (trigger == null)
						trigger = obj.AddComponent<EventTrigger>();

				EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
				entry.callback.AddListener(action);
				trigger.triggers.Add(entry);
		}


		private void OnGrenadeEnter(BaseEventData data) => isTouchOverButton = true;
		private void OnGrenadeExit(BaseEventData data) => isTouchOverButton = false;

		private void OnDisable()
		{
				isInit = false;
		}
}
