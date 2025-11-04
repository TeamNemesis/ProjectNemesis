using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField pwInput;

    public GameObject mainCanvas;
    public GameObject popUpPanel;
    public TextMeshProUGUI popUpMsg;
    public GameObject loginPanel;
    public GameObject loadingPanel;
    public Button resendEmailBtn;
    public Button linkEmailBtn;
    public Button googleLoginBtn;
    public Button popUpConfirmBtn; // 확인 버튼 연결
    public Button logoutBtn;

    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private DatabaseReference dbRef;

    
    private bool shouldChangeScene = false;

    public void Initialize()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        resendEmailBtn.gameObject.SetActive(false);

        bool isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

        googleLoginBtn.gameObject.SetActive(isMobile);
        linkEmailBtn.gameObject.SetActive(isMobile);

        if (isMobile)
        {
            googleLoginBtn.onClick.AddListener(OnClickGoogleLogin);
            linkEmailBtn.onClick.AddListener(() => {
                _ = LinkEmailToGoogleAccount();
            });
        }

        


        if (auth.CurrentUser != null)
        {
            currentUser = auth.CurrentUser;

            if (!currentUser.IsEmailVerified)
            {
                ShowPopup("이메일 인증이 완료되지 않았습니다. 인증 후 다시 로그인해주세요.");
                auth.SignOut();
                return;
            }

            ShowPopup("자동 로그인되었습니다. 계속 진행하려면 확인 버튼을 눌러주세요.", true); // 자동 로그인 후 씬 전환
        }
    }

    public async void OnClickSignUp()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(pwInput.text))
        {
            ShowPopup("아이디와 패스워드를 모두 입력해주세요");
            return;
        }

        SetLoading(true);
        try
        {
            var registerTask = await auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, pwInput.text);
            currentUser = registerTask.User;

            await currentUser.SendEmailVerificationAsync();
            ShowPopup("회원가입이 완료되었습니다. 이메일 인증을 진행해주세요.");

            var userData = new Dictionary<string, object>
            {
                { "playerStatJson", "" },
                { "updateTime", ServerValue.Timestamp }
            };
            await dbRef.Child("playerStats").Child(currentUser.UserId).SetValueAsync(userData);

            await DownloadJsonToLocal(fromGameBase: true);
        }
        catch (System.Exception ex)
        {
            ShowError("회원가입 중 오류가 발생했습니다. 잠시 후 다시 시도해주세요.", "회원가입 오류: " + ex.Message);
        }

        SetLoading(false);
    }

    public async void OnClickLogin()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(pwInput.text))
        {
            ShowPopup("아이디와 패스워드를 모두 입력해주세요");
            return;
        }

        SetLoading(true);
        try
        {
            var loginTask = await auth.SignInWithEmailAndPasswordAsync(emailInput.text, pwInput.text);
            currentUser = loginTask.User;

            await currentUser.ReloadAsync();

            if (!currentUser.IsEmailVerified)
            {
                ShowPopup("이메일 인증이 완료되지 않았습니다. 인증 후 다시 로그인해주세요.");
                resendEmailBtn.gameObject.SetActive(true);
                auth.SignOut();
                SetLoading(false);
                return;
            }

            ShowPopup("로그인 성공", true); // 로그인 후 씬 전환
            resendEmailBtn.gameObject.SetActive(false);

            await DownloadJsonToLocal(fromGameBase: false);
        }
        catch (System.Exception ex)
        {
            ShowError("로그인 중 오류가 발생했습니다. 잠시 후 다시 시도해주세요.", "로그인 오류: " + ex.Message);
        }

        SetLoading(false);
    }

    public async void OnClickResendEmail()
    {
        SetLoading(true);
        try
        {
            var loginTask = await auth.SignInWithEmailAndPasswordAsync(emailInput.text, pwInput.text);
            currentUser = loginTask.User;

            await currentUser.SendEmailVerificationAsync();
            ShowPopup("인증 메일을 다시 전송했습니다. 이메일을 확인해주세요.");
            auth.SignOut();
        }
        catch (System.Exception ex)
        {
            ShowError("인증 메일 재전송 중 오류가 발생했습니다.", "재전송 오류: " + ex.Message);
        }

        SetLoading(false);
    }

    public async void UploadPlayerStatFromLocal()
    {
        SetLoading(true);

        if (currentUser == null)
        {
            ShowPopup("로그인된 사용자가 없어 업로드할 수 없습니다.");
            SetLoading(false);
            return;
        }

        if (!File.Exists(Constants.FILE_PATH_PLAYERSTAT))
        {
            ShowPopup("업로드할 JSON 파일이 존재하지 않습니다.");
            SetLoading(false);
            return;
        }

        string jsonText = File.ReadAllText(Constants.FILE_PATH_PLAYERSTAT);

        var data = new Dictionary<string, object>
        {
            { "playerStatJson", jsonText },
            { "updateTime", ServerValue.Timestamp }
        };

        await dbRef.Child("playerStats").Child(currentUser.UserId).UpdateChildrenAsync(data);

        Debug.Log("JSON 파일을 성공적으로 업로드했습니다.");
        SetLoading(false);
    }

    public async void DownloadPlayerStatToLocal()
    {
        await DownloadJsonToLocal(fromGameBase: false);
    }

    private async Task DownloadJsonToLocal(bool fromGameBase)
    {
        SetLoading(true);

        string jsonText = null;

        try
        {
            if (fromGameBase)
            {
                var snapshot = await dbRef.Child("gameBaseJson").GetValueAsync();
                if (snapshot != null && snapshot.Exists)
                {
                    jsonText = snapshot.GetRawJsonValue();
                }
                else
                {
                    ShowPopup("초기 게임 데이터가 Firebase에 없습니다.");
                }
            }
            else
            {
                if (currentUser == null)
                {
                    ShowPopup("로그인된 사용자가 없습니다.");
                    SetLoading(false);
                    return;

                }

                var snapshot = await dbRef.Child("playerStats").Child(currentUser.UserId).GetValueAsync();
                if (snapshot != null && snapshot.Exists && snapshot.HasChild("playerStatJson"))
                {
                    jsonText = snapshot.Child("playerStatJson").Value.ToString();
                }
                else
                {
                    ShowPopup("사용자 데이터가 Firebase에 없습니다.");
                }
            }

            if (!string.IsNullOrEmpty(jsonText) && jsonText != "null")
            {
                string folderPath = Path.GetDirectoryName(Constants.FILE_PATH_PLAYERSTAT);
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                File.WriteAllText(Constants.FILE_PATH_PLAYERSTAT, jsonText);
                Debug.Log("JSON 데이터를 성공적으로 저장했습니다.");
            }

            UploadPlayerStatFromLocal();
        }
        catch (System.Exception ex)
        {
            ShowError("데이터 다운로드 중 오류가 발생했습니다.", "DownloadJson 오류: " + ex.Message);
        }

        SetLoading(false);
    }

    public async void DeleteAccount()
    {
        SetLoading(true);

        if (currentUser == null)
        {
            ShowPopup("로그인된 사용자가 없어 계정을 삭제할 수 없습니다.");
            SetLoading(false);
            return;
        }

        await dbRef.Child("playerStats").Child(currentUser.UserId).RemoveValueAsync();

        try
        {
            await currentUser.DeleteAsync();

            if (File.Exists(Constants.FILE_PATH_PLAYERSTAT))
            {
                File.Delete(Constants.FILE_PATH_PLAYERSTAT);
            }

            ShowPopup("계정이 성공적으로 삭제되었습니다.");
            SceneManager.LoadScene(0);
        }
        catch (System.Exception ex)
        {
            ShowError("계정 삭제 중 오류가 발생했습니다.", "계정 삭제 오류: " + ex.Message);
        }

        SetLoading(false);
    }


    public async Task LinkEmailToGoogleAccount()
    {
        // 2. 이메일과 비밀번호 입력 확인
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(pwInput.text))
        {
            ShowPopup("이메일과 비밀번호를 입력해주세요.");
            return;
        }

        // 로그인된 사용자 확인
        if (auth.CurrentUser == null)
        {
            ShowPopup("로그인된 사용자가 없습니다.");
            return;
        }

        // 3. 구글 로그인 여부 확인
        bool isGoogleLinked = false;
        bool isEmailLinked = false;

        foreach (var info in auth.CurrentUser.ProviderData)
        {
            if (info.ProviderId == "google.com")
            {
                isGoogleLinked = true;
            }
            else if (info.ProviderId == "password")
            {
                isEmailLinked = true;
            }
        }

        if (!isGoogleLinked)
        {
            ShowPopup("Google 로그인된 상태에서만 이메일 연결이 가능합니다.");
            return;
        }

        if (isEmailLinked)
        {
            ShowPopup("이미 이메일 계정이 연결되어 있습니다.");
            return;
        }

        // 4. 로딩 및 연결 시도
        SetLoading(true);
        ShowPopup("이메일 연결 중입니다. 잠시만 기다려주세요...");

        try
        {
            var emailCredential = EmailAuthProvider.GetCredential(emailInput.text, pwInput.text);
            var linkTask = await auth.CurrentUser.LinkWithCredentialAsync(emailCredential);

            currentUser = linkTask.User;
            ShowPopup(" 이메일 계정이 Google 계정에 성공적으로 연결되었습니다.");
        }
        catch (System.Exception ex)
        {
            ShowError(" 이메일 연결 중 오류가 발생했습니다.", "이메일 연결 오류: " + ex.Message);
        }

        SetLoading(false);
    }

    public void OnClickGoogleLogin()
    {
#if UNITY_ANDROID
        SetLoading(true);

        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("startGoogleLogin"); // Java에서 로그인 시작
        }
        catch (System.Exception ex)
        {
            ShowError("Google 로그인 호출 중 오류가 발생했습니다.", "Android 호출 오류: " + ex.Message);
            SetLoading(false);
        }
#endif
    }

		public async void OnGoogleLoginSuccess(string idToken)
		{
				SetLoading(true);

				try
				{
						Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
						FirebaseUser firebaseUser = await auth.SignInWithCredentialAsync(credential);

						currentUser = firebaseUser;
						ShowPopup("Google 로그인 성공", true);

						await DownloadJsonToLocal(fromGameBase: false);
				}
				catch (System.Exception ex)
				{
						ShowError("Google 로그인 중 오류가 발생했습니다.", "Google 로그인 오류: " + ex.Message);
						auth.SignOut();
				}

				SetLoading(false);
		}

		private void ShowPopup(string message, bool changeSceneOnConfirm = false)
    {
        shouldChangeScene = changeSceneOnConfirm;

        popUpMsg.text = message;
        popUpPanel.SetActive(true);
        Debug.Log(message);

        if (popUpConfirmBtn != null)
        {
            popUpConfirmBtn.gameObject.SetActive(true);
            popUpConfirmBtn.onClick.RemoveAllListeners();
            popUpConfirmBtn.onClick.AddListener(() =>
            {
                popUpPanel.SetActive(false);
                popUpConfirmBtn.gameObject.SetActive(false);

                if (shouldChangeScene)
                {
                    SceneManager.LoadScene(1);
                    mainCanvas.SetActive(false);
                }
            });
        }

        // ✅ 자동 로그인 안내일 경우 로그아웃 버튼 활성화
        if (message.Contains("자동 로그인"))
        {
            logoutBtn.gameObject.SetActive(true);
            logoutBtn.onClick.RemoveAllListeners();
            logoutBtn.onClick.AddListener(() =>
            {
                auth.SignOut();
                logoutBtn.gameObject.SetActive(false);
                popUpPanel.SetActive(false);
                ShowPopup("로그아웃되었습니다. 다시 로그인해주세요.");
            });
        }
        else
        {
            logoutBtn.gameObject.SetActive(false);
        }
    }


    private void ShowError(string userMessage, string logMessage)
    {
        ShowPopup(userMessage);
        Debug.LogError(logMessage);
    }

    private void SetLoading(bool isActive)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(isActive);
    }

    public void OnClickSceneBtn()
    {
        SceneManager.LoadScene(1);
    }


}
