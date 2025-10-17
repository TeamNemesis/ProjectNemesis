using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour,IPoolable
{
		/// <summary>
		/// 배치된 스킬 데이터
		/// </summary>
		private SkillData _skillData;
		public SkillData skillData { get { return _skillData; } }

		public void SetSkillData(SkillData setSkillData)
		{
				_skillData = setSkillData;
		}

		[SerializeField]
		private Text _skillImage;
		[SerializeField]
		private Text _skillScirpt;
		[SerializeField]
		private Text _skillIDX;
		[SerializeField]
		private Text _skillLevel;


		public void SetSkillInfo(SkillData choosedSkill)
		{
				_skillData = choosedSkill;



				if (_skillScirpt != null)
				{
						_skillScirpt.text = choosedSkill.skillScript;
				}
				if (_skillImage != null)
				{
						_skillImage.text = choosedSkill.skillImagePath;
				}
				if (_skillIDX != null)
				{
						_skillIDX.text = choosedSkill.skillIdx.ToString();
				}
				if(_skillLevel !=null)
				{
						_skillLevel.text = choosedSkill.skillLevel.ToString() + " / " + choosedSkill.skillMaxLevel.ToString();
				}
		}

    public void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetGameObject()
    {
		return gameObject;
    }

    public void ReleaseObject()
    {
        throw new System.NotImplementedException();
    }
}