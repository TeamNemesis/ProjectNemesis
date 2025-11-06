using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] GameObject introPanel;
    [SerializeField] GameObject playPanel;


    public void OnOpenSetting()
    {
        if(SceneManager.GetActiveScene().buildIndex !=1)
        {
            introPanel.SetActive(true);
        }
        else
        {
            playPanel.SetActive(true);
        }
    }


    public void OnBtnClick_GameExit()
    {
        Application.Quit();
    }

    public void OnBtnClick_Re()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
