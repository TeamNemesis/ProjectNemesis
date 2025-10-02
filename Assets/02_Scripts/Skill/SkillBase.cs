using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
		/// <summary>
		/// 플레이어가 가지고 있는 스킬 종류
		/// </summary>
		public int currentSkillCnt;

		/// <summary>
		/// 회사 스킬 레벨
		/// </summary>
		public Dictionary<int, SkillData> skillLevel = new Dictionary<int, SkillData>();

		public string skillDataPath;

		/// <summary>
		/// 회사 스킬 레벨 초기화
		/// </summary>
		public void InitSkillDictionary()
		{

				for (int i = 0; i < 10; i++)
				{
						skillLevel[i] = new SkillData(i + 1, skillDataPath);
				}
		}

}

public class SkillData
{
		private List<Dictionary<string, object>> skillCSVInfo;

		public int skillIdx;

		public string skillScript;

		public string skillImagePath;

		public int skillLevel;

		/// <summary>
		/// 초기화 용
		/// </summary>
		/// <param name="skillDataPath"></param>
		public SkillData(int idx, string skillDataPath)
		{
				skillCSVInfo = CSVReader.Read(skillDataPath);
				skillIdx = idx;

				skillScript = skillCSVInfo[idx - 1]["SCRIPT"].ToString();
				skillImagePath = skillCSVInfo[idx - 1]["IMAGE"].ToString();

				skillLevel = 0;
				Debug.Log("초기화");
		}

		public void LevelUp()
		{
				skillLevel++;
				Debug.Log("레벨업" +  skillLevel);	

		}


}
