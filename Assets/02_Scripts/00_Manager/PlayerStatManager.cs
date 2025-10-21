using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
		#region 공격력 

		#region 검
		/// <summary>
		/// 검 일반 공격력
		/// </summary>
		private float _bladeAttackDamage;
		public float bladeAttackDamage;
		public void AddBladeAttackDamage(float plusBladeAttack)
		{
				_bladeAttackDamage += plusBladeAttack;
		}


		/// <summary>
		/// 검 특수 공격력
		/// </summary>
		private float _bladeSPAttackDamage;
		public float bladeSPAttackDamage { get { return _bladeAttackDamage; } }
		public void AddBladeSPAttackDamage(float plusBladeSPAttack)
		{
				_bladeSPAttackDamage += plusBladeSPAttack;
		}

		#endregion
		#region 라이플
		/// <summary>
		/// 원거리 일반 공격력
		/// </summary>
		private float _playerRifleAttackDamage;
		public float playerRifleAttackDamage;
		public void AddPlayerRifleAttackDamage(float plusPlayerRifleAttack)
		{
				_playerRifleAttackDamage += plusPlayerRifleAttack;
		}

		/// <summary>
		/// 원거리 특수 공격력
		/// </summary>
		private float _playerRifleSPAttackDamage;
		public float playerRifleSPAttackDamage;
		public void AddPlayerRifleSPAttackDamage(float plusPlayerRifleSPAttackDamage)
		{
				_playerRifleSPAttackDamage += plusPlayerRifleSPAttackDamage;
		}

		/// <summary>
		/// 플레이어 블렛 이동 속도
		/// </summary>
		private float _playerBulletMoveSpeed;
		public float playerBulletMoveSpeed;
		public void AddPlayerBulletMoveSpeed(float plusPlayerBulletMoveSpeed)
		{
				_playerBulletMoveSpeed += plusPlayerBulletMoveSpeed;
		}

		/// <summary>
		/// 플레이어 블렛 지속 시간
		/// </summary>
		private float _playerBulletLifeTime;
		public float playerBulletLifeTime;
		public void AddPlayerBulletLifeTime(float plusPlayerBulletLifeTime)
		{
				_playerBulletLifeTime += plusPlayerBulletLifeTime;
		}

		#endregion
		#region 특수장비
		/// <summary>
		/// 특수장비 일반 공격력
		/// </summary>
		private float _playerHackAttackDamage;
		public float playerHackAttackDamage;
		public void AddPlayerHackAttackDamage(float plusPlayerHackAttackDamage)
		{
				_playerHackAttackDamage += plusPlayerHackAttackDamage;
		}

		/// <summary>
		/// 특수장비 특수 공격력
		/// </summary>
		private float _playerHackSPAttackDamage;
		public float playerHackSPAttackDamage;
		public void AddPlayerHackSPAttackDamage(float plusPlayerHackSPAttackDamage)
		{
				_playerHackSPAttackDamage += plusPlayerHackSPAttackDamage;
		}
		#endregion
		/// <summary>
		/// 플레이어 일반 공격 데미지 계수
		/// </summary>
		private float _playerAttackDamage;
		public float playerAttackDamage { get { return _playerAttackDamage; } }
		public void AddPlayerAttackDamage(float plusDamage)
		{
				_playerAttackDamage += plusDamage;
		}



		/// <summary>
		/// 플레이어 유탄 공격 데미지 계수
		/// </summary>
		private float _playerGrenadeDamage;
		public float playerGrenadeDamage { get { return _playerGrenadeDamage; } }
		public void AddPlayerGrenadeDamage(float plusGrenadeDamage)
		{
				_playerAttackDamage += plusGrenadeDamage;
		}

		/// <summary>
		/// 플레이어 특수 공격 데미지 계수
		/// </summary>
		private float _playerSPAttackDamage;
		public float playerSPAttackDamage { get { return _playerSPAttackDamage; } }
		public void AddPlayerSPAttackDamage(float plusSPDamage)
		{
				_playerSPAttackDamage += plusSPDamage;
		}

		/// <summary>
		/// 플레이어 대쉬 공격력 계수
		/// </summary>
		private float _playerDashDamage;
		public float playerDashDamage { get { return _playerDashDamage; } }
		public void AddPlayerDashDamage(float plusDashDamage)
		{
				_playerDashDamage += plusDashDamage;
		}

		#endregion

		#region 모든 데미지, 이동속도, 애니메이션 재생속도

		/// <summary>
		/// 모든 데미지 증가 계수
		/// </summary>
		private float _totalMultiDamage;
		public float totalMultiDamage { get { return _totalMultiDamage; } }

		/// <summary>
		/// 모든 데미지 계수 증가
		/// </summary>
		/// <param name="addDamage">ex)0.5배</param>
		public void AddTotalMultiDamage(float addDamage)
		{
				_totalMultiDamage += addDamage;
		}
		/// <summary>
		/// 플레이어 이동 속도 계수
		/// </summary>
		private float _playerMoveSpeed;
		public  float playerMoveSpeed { get { return _playerMoveSpeed; } }
		public  void AddPlayerMoveSpeed(float plusMoveSpeed)
		{
				_playerMoveSpeed += plusMoveSpeed;
		}

		/// <summary>
		/// 플레이어 대쉬 거리
		/// </summary>
		private float _playerDashDistance;
		public float playerDashDistance { get { return _playerDashDistance; } }
		public void AddPlayerDashDistance(float plusDashDistance)
		{
				_playerDashDistance += plusDashDistance;
		}

		/// <summary>
		/// 플레이어 대쉬거리 곱 계수
		/// </summary>
		private float _playerDashDistanceMulti;
		public float playerDashDistanceMulti { get { return _playerDashDistanceMulti; } }
		public void AddPlayerDashDistanceMulti(float plusDashDistanceMulti)
		{
				_playerDashDistanceMulti += plusDashDistanceMulti;
		}

		/// <summary>
		/// 플레이어 애니메이션 일반 공격 재생 속도
		/// </summary>
		private float _playerAttackAnimSpeed;
		public float playerAttackAnimSpeed;
		public void AddPlayerAttackAnimSpeed(float plusAttackAnimSpeed)
		{
				_playerAttackAnimSpeed += plusAttackAnimSpeed;
		}


		#endregion

		#region 유탄, 범위공격

		/// <summary>
		/// 플레이어 범위 공격 범위 계수
		/// </summary>
		private float _playerAreaExtent = 1f;
		public float playerAreaExtent { get { return _playerAreaExtent; } }
		public void AddPlayerAreaExtent(float plusAreaExtent)
		{
				_playerAreaExtent += plusAreaExtent;
		}


		/// <summary>
		/// 유탄 쿨타임 계수
		/// </summary>
		private float _grenadeCoolTime;
		public float grenadeCoolTime { get { return _grenadeCoolTime; } }
		public void SetGrenadeCoolTime(float grenadeCoolTime)
		{
				_grenadeCoolTime *= grenadeCoolTime;
		}

		#endregion

		#region 받는 피해
		/// <summary>
		/// 플레이어 받는 피해 계수
		/// </summary>
		private float _playerHitPercent = 1f;
		public float playerHitPercent { get { return _playerHitPercent; } }
		public void AddPlayerHitPercent(float plusHitPercent)
		{
				_playerHitPercent += plusHitPercent;
		}

		/// <summary>
		/// 플레이어 회피율
		/// </summary>
		private float _playerAvoidance;
		public float playerAvoidance {  get { return _playerAvoidance; } }
		public void AddPlayerAvoidance(float plusPlayerAvoidance)
		{
				_playerAvoidance += plusPlayerAvoidance;		
		}
		#endregion

		#region 넉백
		/// <summary>
		/// 넉백 충돌 데미지 계수 
		/// </summary>
		private float _knockBackDamage;
		public float knockBackDamage;
		public void AddKockBackDamage(float plusKnockBackDamage)
		{
				_knockBackDamage += plusKnockBackDamage;
		}

		/// <summary>
		/// 넉백 거리 계수
		/// </summary>
		private float _knockBackDistance;
		public float knockBackDistance;
		public void AddKnockBackDistance(float plusKnockBackDistance)
		{
				_knockBackDistance += plusKnockBackDistance;
		}
		#endregion

		#region 추가데미지
		/// <summary>
		/// 취약, 디버프 있으면 추가 데미지 계수
		/// </summary>
		private float _debuffPlusDamage;
		public float debuffPlusDamage;
		public void AddDebuffPlusDamage(float plusDebuffPlusDamage)
		{
				_debuffPlusDamage += plusDebuffPlusDamage;
		}

		/// <summary>
		/// 약화 추가 데미지 계수
		/// </summary>
		private float _weakenPlusDamage;
		public float weakenPlusDamage { get { return _weakenPlusDamage; } }
		public void AddWeakenPlusDamage(float PlusWeakenDamage)
		{
				_weakenPlusDamage += PlusWeakenDamage;
		}
		#endregion
}
