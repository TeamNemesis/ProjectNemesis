using System;
using UnityEngine;

/// <summary>
/// 비브르 강화 대쉬 강화 (약육강식)
/// </summary>
public class Skill_One_Dash : ActiveTech
{

		public override event Action OnTechUsed;

		/// <summary>
		/// 대쉬 시작 독 프리팹
		/// </summary>
		[SerializeField]


		public override void Activate(SkillManager skillManager, PlayerModel player)
		{
				// 공격 적중 시 이벤트에 추가
				base.Activate(skillManager, player);
				//TODO 대쉬 입력 시 이벤트에 연결

		}
		public override void Deactivate(PlayerModel player)
		{
				// 리스트 제거
				base.Deactivate(player);
				// 이벤트 해제
				//TODO 대쉬 입력 시 이벤트에 해제

		}

		public override void AttackTry(PlayerModel player)
		{
				Vector3 position = player.transform.position;
				position.y = 0;
				//ObjectPool.Instantiate()
		}


		public Skill_One_Dash(SkillData skillData) : base(skillData)
		{
		}
}

/// <summary>
/// 파이로 하트 대쉬 강화 (깜짝선물)
/// </summary>
public class Skill_Two_Dash : ActiveTech
{
		private DashReinforcePrefab _dashReinforcePrefab;

		public Action _AttackTry;

		public override event Action OnTechUsed;

		/// <summary>
		/// 착탄 지점 독 프리팹
		/// </summary>
		[SerializeField]

		public override void Activate(SkillManager skillManager, PlayerModel player)
		{
				// 공격 적중 시 이벤트에 추가
				base.Activate(skillManager, player);
				_dashReinforcePrefab = Resources.Load<DashReinforcePrefab>("Prefabs/Skill/SkillObject/Skill_Two/SkillTwoDash");
				_AttackTry = () => AttackTry(player);
				//TODO 대쉬 시작 이벤트에 연결

		}
		public override void Deactivate(PlayerModel player)
		{
				// 리스트 제거
				base.Deactivate(player);
				// 이벤트 해제
				//TODO 대쉬 시작시 이벤트에서 해제 
		}

		public override void AttackTry(PlayerModel player)
		{
				Vector3 position = player.transform.position;
				position.y = 0;
				ObjectPool.Instance.GetFromPool<DashReinforcePrefab>(_dashReinforcePrefab, position);
		}

		public Skill_Two_Dash(SkillData skillData) : base(skillData)
		{
		}
}