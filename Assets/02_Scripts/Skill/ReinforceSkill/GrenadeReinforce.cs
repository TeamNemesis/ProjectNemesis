using System;
using UnityEngine;

/// <summary>
/// 비브르 강화 유탄 강화 (포자 퍼뜨리기)
/// </summary>
public class Skill_One_Grenade : ActiveTech
{

		public override event Action OnTechUsed;

		/// <summary>
		/// 착탄 지점 독 프리팹
		/// </summary>
		[SerializeField]
		private GrenadePoison _grenadePoisonPrefab;

		public override void Activate(SkillManager skillManager, PlayerModel player)
		{
				// 공격 적중 시 이벤트에 추가
				base.Activate(skillManager, player);

				_grenadePoisonPrefab = Resources.Load<GrenadePoison>("Prefabs/Skill/SkillObject/Skill_One/GrenadePoison");
				//TODO 유탄 폭발시 이벤트에 연결
				player.GrenadeBomb += GrenadeBomb;

		}
		public override void Deactivate(PlayerModel player, bool isSameSkill)
		{
				// 리스트 제거
				base.Deactivate(player, isSameSkill);
				// 이벤트 해제
				//TODO 유탄 폭발시 이벤트에서 해제
				player.GrenadeBomb -= GrenadeBomb;
		}

		public void GrenadeBomb(Vector3 position)
		{
				position.y = 0;
				ObjectPool.Instance.GetFromPool(_grenadePoisonPrefab, position, _grenadePoisonPrefab.transform.rotation).GetComponent<GrenadePoison>().Initialize();

		}


		public Skill_One_Grenade(SkillData choosedSkill) : base(choosedSkill)
		{

		}

		
}

/// <summary>
/// 파이로 하트 유탄공격 강화
/// </summary>
public class Skill_Two_Grenade : ActiveTech
{
	
		public override event Action OnTechUsed;

		private int originalGrenadeAttack = 100;
		public override void Activate(SkillManager skillManager, PlayerModel player)
		{
				base.Activate(skillManager, player);
				#region Test

				// 스킬 효과 적용 (플레이어 유탄 공격력에 접근하여 공격력 추가)
				player.playerAttack = (int)(originalGrenadeAttack * ((float)_skillData.skillBaseValue_1 + (float)_skillData.skillLevelValue_1 * _skillData.skillLevel));
				// player.playerAttack = (일반 공격 증가 식)
				// 공격 적중 시 이벤트에 추가

		}
	public override void Deactivate(PlayerModel player, bool isSameSkill)
		{
				// 리스트 제거
				base.Deactivate(player, isSameSkill);
				// 유탄공격력 복귀
				player.playerAttack = originalGrenadeAttack;
	

		}
		#endregion


		public Skill_Two_Grenade(SkillData skillData) : base(skillData)
		{
		}
}