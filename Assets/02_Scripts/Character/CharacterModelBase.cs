using System;
using System.Transactions;
using UnityEngine;

/// <summary>
/// 몬스터와 플레이어 부모 클래스
/// </summary>
public abstract class CharacterModelBase : MonoBehaviour,IDamageable
{
    [SerializeField] protected int maxHealth = 100;
		public int MaxHp => maxHealth; // 최대 체력을 반환하는 속성

		[SerializeField] protected int currentHealth;
		public int CurrentHp => currentHealth; // 현재 체력을 반환하는 속성

		[SerializeField] float _moveSpeed = 5; // 이동 속도
		public float moveSpeed { get { return _moveSpeed; } }
				public void SetMoveSpeed(float speed)
		{
				_moveSpeed = speed;
				OnMoveSpeedChanged?.Invoke(_moveSpeed);
		}


		[Header("상태이상")]
		[SerializeField] public bool isStunned = false;
		[SerializeField] public bool isPushed = false;


		[SerializeField] public bool isDead = false;

		public event Action<int> OnHpChanged; // 체력 변경 시 발생하는 이벤트

		public event Action<float> OnMoveSpeedChanged; // 이동 속도 변경 시 발생하는 이벤트

		public event Action OnDieEvent;

		public void OnHpChangedEventPlay(int currentHp)
		{
				OnHpChanged?.Invoke(currentHp);		
		}

		[SerializeField] protected DebuffHandler debuffHandler;


		/// <summary>
		/// 캐릭터 생성 시 초기화 함수
		/// </summary>
		public virtual void Initialize()
		{
        debuffHandler = GetComponent<DebuffHandler>();

		}

		public void TakeDamage(float damage)
		{
				if (isDead) return;

				// 과부하 디버프
				if (debuffHandler != null && debuffHandler.HasDebuff(Constants.DEBUFF_OVERLOAD))
				{
						int stacks = debuffHandler.GetStackCount(Constants.DEBUFF_OVERLOAD);
						float bonus = 1f + (0.05f * stacks);
						damage *= bonus;
				}

				// 화상 디버프
				if (debuffHandler != null && debuffHandler.HasDebuff(Constants.DEBUFF_BURN))
				{
						damage *= 2f;
				}

				currentHealth -= (int)damage;
				if (currentHealth <= 0)
				{
						currentHealth = 0;
						Die();
				}
				else
				{
						OnHpChanged?.Invoke(currentHealth); // 체력 변경 이벤트 발행
				}
		}

		

		protected virtual void Die()
		{
				OnDieEvent?.Invoke();

				isDead = true;
				Destroy(gameObject);
		}
}
