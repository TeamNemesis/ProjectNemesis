using System.Collections;
using UnityEngine;

/// <summary>
/// 추후 비브르 기술로 변경
/// </summary>
public class Skill_One : SkillBase
{
		private Coroutine _autoHeal;

		/// <summary>
		/// 피격시 생성할 독 프리팹
		/// </summary>
		[SerializeField]
		private PoisonSpread _hitPoisonSpreadPrefab;

		/// <summary>
		/// 진화에 필요한 스택
		/// </summary>
		private int levelupStack;



		public override void ActivateSkill(SkillData choosedSkill)
		{
				switch (choosedSkill.skillIdx)
				{
						// 독사의 앞니
						case 10:
								Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								ActiveTech skillAttack = new Skill_One_Attack(choosedSkill);
								if (_skillManager.attackTech != null)
								{
										if (_skillManager.attackTech.skillData.skillIdx != choosedSkill.skillIdx)
										{
												_skillManager.attackTech.Deactivate(player);
										}
								}
								skillAttack.Activate(_skillManager, player);
								break;

						// 포자 퍼뜨리기
						case 11:
								Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								ActiveTech skillGrenade = new Skill_One_Grenade(choosedSkill);
								if (_skillManager.bombTech != null)
								{
										if (_skillManager.bombTech.skillData.skillIdx != choosedSkill.skillIdx)
										{
												_skillManager.bombTech.Deactivate(player);
										}
								}
								skillGrenade.Activate(_skillManager, player);
								break;

						// 피의 갈증
						case 12:
								Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								ActiveTech skillSPAttack = new Skill_One_SPAttack(choosedSkill);
								if (_skillManager.skillTech != null)
								{
										if (_skillManager.skillTech.skillData.skillIdx != choosedSkill.skillIdx)
										{
												_skillManager.skillTech.Deactivate(player);
										}
								}
								skillSPAttack.Activate(_skillManager, player);
								break;

						// 약육강식
						case 13:
								Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								ActiveTech skillDash = new Skill_One_Dash(choosedSkill);
								if (_skillManager.dashTech != null)
								{
										if (_skillManager.dashTech.skillData.skillIdx != choosedSkill.skillIdx)
										{
												_skillManager.dashTech.Deactivate(player);
										}
								}
								break;

						// 넘치는 활력
						case 14:
								Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								player.SetMaxHp((int)choosedSkill.skillLevelValue_1);
								break;

						// 초재생
						case 15:
								Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								//TODO 방 입장시 이벤트에 초재생 연결 StartAutoHeal()
								//TODO 전투 종료시 이벤트에 초재생 해제 연결 StopAutoHeal()
								break;

						// 독성혈액
						case 16:
								Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								//TODO 플레이어 모델에 받는데미지 감소 계수를 추가하여 10퍼센트 

								//TODO 피격시 이벤트에 함수 추가 SpreadPoison
								player.PlayerHit += ()=>SpreadPoison(player);
								break;

						// 진화
						case 17:
								Debug.Log($"{choosedSkill.skillIdx} 발동, 스킬 레벨 : {choosedSkill.skillLevel}");
								//TODO 문과 상호작용시에 연결 SkillLevelUp
								// 연결시 SkillLevelUp이 완료되면 다음 과정으로 넘어갈 수 있게
								break;

						default:
								Debug.Log("에러, 배정되지 않은 idx");
								break;
				}

		}

		#region 초재생

		//초재생 코루틴
		public IEnumerator StartAutoHealRoutine()
		{
				int stack = 0;
				while (stack < 20)
				{
						yield return new WaitForSeconds(Constants.HEAL_SECOND);
						player.Heal(Constants.HEAL_AMOUNT);
						stack++;
				}
		}


		/// <summary>
		/// 초재생 실행
		/// </summary>
		public void StartAutoHeal()
		{
				_autoHeal = StartCoroutine(StartAutoHealRoutine());
		}

		/// <summary>
		/// 초재생 종료
		/// </summary>
		public void StopAutoHeal()
		{
				StopCoroutine(_autoHeal);
				_autoHeal = null;
		}
		#endregion

		#region 독성혈액

		public void SpreadPoison(PlayerModel player)
		{
				Vector3 position = player.transform.position;
				position.y = 0;
				//TODO 스킬 확인
				PoisonSpread poisonSpread = ObjectPool.Instance.GetFromPool<PoisonSpread>(_hitPoisonSpreadPrefab,position);

		}



		#endregion

		#region 진화
		public void SkillLevelUp()
		{
				++levelupStack;
				if (levelupStack >= 2)
				{
						// 업그레이드 가능 스킬이 없다면 리턴
						if (!(_skillManager.upgradeSkillList.Count > 0))
						{
								return;
						}
						// 랜덤한 스킬 레벨 업
						_skillManager.upgradeSkillList[Random.Range(0, _skillManager.upgradeSkillList.Count)].ChooseSkill();

						// 스택 초기화
						levelupStack = 0;
				}
		}
		#endregion



}


