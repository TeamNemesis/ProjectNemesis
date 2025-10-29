using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillChoose))]
public class SkillChooseEditor : Editor
{
		private int skillIndexToActivate = 0;

		public override void OnInspectorGUI()
		{
				// 기본 인스펙터 표시
				DrawDefaultInspector();

				SkillChoose skillChoose = (SkillChoose)target;

				GUILayout.Space(10);
				GUILayout.Label("🔧 디버그 기능", EditorStyles.boldLabel);

				// 일반 스킬 선택 버튼
				if (GUILayout.Button("SetBtn() - 일반 스킬 선택"))
				{
						skillChoose.SetBtn();
				}

				// 돌연변이 스킬 선택 버튼
				if (GUILayout.Button("SetMutant() - 돌연변이 스킬 선택"))
				{
						skillChoose.SetMutant();
				}

				// 업그레이드 스킬 선택 버튼
				if (GUILayout.Button("SetUpgradeBtn() - 업그레이드 스킬 선택"))
				{
						skillChoose.SetUpgradeBtn();
				}

				GUILayout.Space(10);
				GUILayout.Label("🎯 스킬 인덱스로 Activate", EditorStyles.boldLabel);

				// 인덱스 입력 필드
				skillIndexToActivate = EditorGUILayout.IntField("스킬 인덱스", skillIndexToActivate);

				// Activate 버튼
				if (GUILayout.Button($"ActivateSkillByIndex({skillIndexToActivate})"))
				{
						GameManager.Instance.skillManager.ActivateSkillByIndex(skillIndexToActivate);
				}
		}



}
