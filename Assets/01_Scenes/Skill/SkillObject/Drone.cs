using System.Collections;
using UnityEngine;

public class Drone : MonoBehaviour
{
		[SerializeField]
		private enum State
		{
				Idle, // 몬스터 감지 못함
				Attack // 공격
		}

		//TODO 플레이어 스탯 필드

		/// <summary>
		/// 현재 상태
		/// </summary>
		[SerializeField]
		private State _currentState = State.Idle;

		/// <summary>
		/// 공격 쿨타임
		/// </summary>
		[SerializeField]
		private float _attackCoolTime = Constants.DRONE_ATTACKDELAY;

		/// <summary>
		/// 사정거리
		/// </summary>
		[SerializeField]
		private float _attackRange = Constants.DRONE_ATTACKRANGE;

		/// <summary>
		/// 탐색 콜라이더 리스트
		/// </summary>
		[SerializeField]
		private Collider[] _results = new Collider[Constants.DRONE_SEARCHNUM];


		/// <summary>
		/// 공격 중인지
		/// </summary>
		[SerializeField]
		private bool _bIsAttacking;

		/// <summary>
		/// 현재 공격 목표
		/// </summary>
		[SerializeField]
		private Transform _currentTarget;
		public Transform currentTarget { get { return _currentTarget; } }

		/// <summary>
		/// 총알 프리팹
		/// </summary>
		[SerializeField]
		private DroneBullet _droneBulletPrefab;


		private void Update()
		{
				//TODO 플레이어 사망시 리턴
				switch (_currentState)
				{
						case State.Idle:
								SearchEnemy();
								break;
						case State.Attack:
								LookTarget();
								if (!_bIsAttacking)
								{
										StartCoroutine(AttackTarget());
								}
								break;
						default:
								break;
				}
		}

		/// <summary>
		/// 타겟 설정 
		/// </summary>
		public void SearchEnemy()
		{
				// 콜라이더 탐색 (필요하다면 레이어마스크 설정)
				int hitColliders = Physics.OverlapSphereNonAlloc(transform.position, _attackRange, _results);

				// 임시 저장할 변수
				MonsterBase targetMonster = null;
				float minDistance = float.MaxValue;

				for (int i = 0; i < hitColliders; i++)
				{
						// Monster인지 판단
						MonsterBase currentMonster = _results[i].GetComponent<MonsterBase>();
						if (currentMonster != null)
						{
								// Monster라면 거리 비교
								float distacne = Vector3.Distance(_results[i].transform.position, transform.position);
								if (distacne < minDistance)
								{
										minDistance = distacne;
										targetMonster = currentMonster;

								}
						}

				}

				// 몬스터가 탐색되었다면 타겟 설정
				if(targetMonster!= null)
				{
						_currentTarget = targetMonster.transform;
						_currentState = State.Attack;
				}

		}

		/// <summary>
		/// 드론이 몬스터를 바라보게
		/// </summary>
		public void LookTarget()
		{
				Vector3 direction = (_currentTarget.position - transform.position).normalized;
				if (direction != Vector3.zero)
				{
						Quaternion targetRotation = Quaternion.LookRotation(direction);
						transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * Constants.DRONE_ROTATION_SPEED);
				}
		}

		/// <summary>
		/// 몬스터 공격
		/// </summary>
		/// <returns></returns>
		public IEnumerator AttackTarget()
		{
				_bIsAttacking = true;
				// 타겟이 존재하고 사정거리 안이라면
				if (_currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) < _attackRange)
				{
						FireBullet();
						yield return new WaitForSeconds(_attackCoolTime);
				}
				_bIsAttacking = false;
				_currentState = State.Idle;
		}

		/// <summary>
		/// 총알 발사
		/// </summary>
		public void FireBullet()
		{
				Vector3 spawnPosition = transform.position + transform.forward;
				Quaternion spawnRotation = transform.rotation;

				DroneBullet bullet = Instantiate(_droneBulletPrefab, spawnPosition, spawnRotation);
				if (bullet != null)
				{
						//TODO 데미지 세팅
				}
		}



}
