using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static SkillManager _instance;  
    
    public static SkillManager Instance()
    {
        return _instance;
    }

		public void Awake()
		{
				if(_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
		}




}
