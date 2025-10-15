using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebuffHandler : MonoBehaviour
{
		public class DebuffData
		{
				public string debuffName;      // 디버프 이름
				public float debuffDuration;   // 지속시간
				public float debuffValue;      // 초당 대미지나 배수값
				public int maxStack;           // 최대 스택
		}

		[SerializeField]
		private class ActiveDebuff
		{
				public DebuffData data;
				public float remainingTime;
				public float totalValue;
				public int stackCount;
				public IEnumerator routine;

				public ActiveDebuff(DebuffData data, IEnumerator routine)
				{
						this.data = data;
						remainingTime = data.debuffDuration;
						totalValue = data.debuffValue;
						stackCount = 1;
						this.routine = routine;
				}
		}


		/// <summary>
		/// 몬스터용 초기화 함수
		/// </summary>
		/// <param name="monsterAgent"></param>
		public void Initialize(NavMeshAgent monsterAgent)
		{
				character = GetComponent<CharacterBase>();
				if (gameObject.CompareTag(Constants.TAG_MONSTER))
				{
						agent = monsterAgent;
				}
		}

		/// <summary>
		/// 플레이어용 초기화 함수
		/// </summary>
		public void Initialize()
		{
				character = GetComponent<CharacterBase>();
				
		}


		[SerializeField]
		private Dictionary<string, ActiveDebuff> activeDebuffs = new Dictionary<string, ActiveDebuff>();
		private CharacterBase character;
		private NavMeshAgent agent;
		/// <summary>
		/// 디버프 적용
		/// </summary>
		public void ApplyDebuff(DebuffData newDebuff)
		{

				if (character == null || character.isDead)
						return;

				if (activeDebuffs.ContainsKey(newDebuff.debuffName))
				{
						ActiveDebuff existing = activeDebuffs[newDebuff.debuffName];

						// 스택형 디버프들
						if (newDebuff.debuffName == Constants.DEBUFF_POISON || newDebuff.debuffName == Constants.DEBUFF_OVERLOAD)
						{
								if (existing.stackCount < newDebuff.maxStack)
								{
										existing.stackCount++;
										existing.totalValue += newDebuff.debuffValue;
								}
								existing.remainingTime = newDebuff.debuffDuration;
						}
						else
						{
								existing.remainingTime = newDebuff.debuffDuration;
								existing.totalValue = newDebuff.debuffValue;
						}

						return;
				}

				IEnumerator routine = (HandleDebuff(newDebuff));
				activeDebuffs.Add(newDebuff.debuffName, new ActiveDebuff(newDebuff, routine));
				StartCoroutine(activeDebuffs[newDebuff.debuffName].routine);
		}

		private IEnumerator HandleDebuff(DebuffData debuff)
		{


				ActiveDebuff active = activeDebuffs[debuff.debuffName];

				// 시작 시 1회 효과
				switch (debuff.debuffName)
				{
						case Constants.DEBUFF_SLOW:
								if (agent != null)
								{
										agent.speed *= 0.7f;

								}
								else
								{
										character.SetMoveSpeed(3.5f);
								}
								break;

						case Constants.DEBUFF_STUN:
								StartCoroutine(StunCoroutine(debuff.debuffDuration));
								break;

						case Constants.DEBUFF_CONFUSION:
								MonsterBase monster = character.GetComponent<MonsterBase>();
								StartCoroutine(ConfuseCoroutine(debuff.debuffDuration, monster));
								break;
				}

				while (active.remainingTime > 0f && !character.isDead)
				{
						switch (debuff.debuffName)
						{
								case Constants.DEBUFF_POISON:
								case Constants.DEBUFF_OVERLOAD:
										Debug.Log("takeDamage" + active.totalValue);
										character.TakeDamage(active.totalValue);
										break;
						}

						active.remainingTime -= 1f;
						yield return new WaitForSeconds(1f);
				}

				// 해제 시 복원
				switch (debuff.debuffName)
				{
						case Constants.DEBUFF_SLOW:
								if (agent != null)
								{
										agent.speed /= 0.7f;
								}
								else
								{
										character.SetMoveSpeed(5f);
								}
								break;
				}

				activeDebuffs.Remove(debuff.debuffName);

		}

		private IEnumerator StunCoroutine(float duration)
		{
				character.isStunned = true;

				if (agent != null)
						agent.isStopped = true;

				yield return new WaitForSeconds(duration);

				if (agent != null && !character.isDead)
						agent.isStopped = false;

				character.isStunned = false;
		}

		/// <summary>
		/// 혼란
		/// </summary>
		/// <param name="duration"></param>
		/// <returns></returns>
		private IEnumerator ConfuseCoroutine(float duration, MonsterBase monster)
		{
				string originalTag = monster.targetTag;
				monster.targetTag = Constants.TAG_MONSTER;

				yield return new WaitForSeconds(duration);

				monster.targetTag = originalTag;
		}

		public bool CheckDebuff(DebuffData data)
		{
				return activeDebuffs.ContainsKey(data.debuffName);
		}

		public bool HasDebuff(string debuffName)
		{
				return activeDebuffs.ContainsKey(debuffName);
		}

		public int GetActiveDebuffCount()
		{
				int count = 0;

				foreach (KeyValuePair<string, ActiveDebuff> pair in activeDebuffs)
				{
						ActiveDebuff debuff = pair.Value;
						if (debuff != null && debuff.remainingTime > 0f)
								count++;
				}

				return count;
		}

		public int GetStackCount(string debuffName)
		{
				if (activeDebuffs.ContainsKey(debuffName))
						return activeDebuffs[debuffName].stackCount;
				return 0;
		}
}
