using System.Collections;
using UnityEngine;

public class DashReinforcePrefab : AreaDamageBase,IPoolable
{
		public override void ActiveSkill(Transform target)
		{
				// 화상 적용
				DebuffHandler targetDebuffHandler = target.GetComponent<DebuffHandler>();
				targetDebuffHandler.ApplyDebuff(DebuffHandler.DebuffData.CreateBurn());
		}

		public GameObject GetGameObject()
		{
				return gameObject;
		}


		public void Initialize()
		{
				CheckTarget();
				StartCoroutine(ReleaseObjectCoroutine());
		}

		public void ReleaseObject()
		{

		}

		IEnumerator ReleaseObjectCoroutine()
		{
				yield return new WaitForSeconds(Constants.SKILL_REMAIN);
				ObjectPool.Instance.ReleaseToPoolByInterface(this);
		}
}
