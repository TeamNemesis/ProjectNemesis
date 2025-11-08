using Firebase;
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
    private FirebaseUser _currentUser;
    private string deviceId;
    public FirebaseUser currentUser { get { return _currentUser; } }
    private DatabaseReference _dbRef;
    public DatabaseReference dbRef { get { return _dbRef; } }

    private bool shouldChangeScene = false;
    private DownloadManager _downloadManager;
    public DownloadManager downloadManager { get { return _downloadManager; } }


    public async void Initialize()
    {
        auth = FirebaseAuth.DefaultInstance;
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        deviceId = SystemInfo.deviceUniqueIdentifier;

        resendEmailBtn.gameObject.SetActive(false);

        bool isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;



        if (isMobile)
        {
            googleLoginBtn.onClick.AddListener(OnClickGoogleLogin);
            linkEmailBtn.onClick.AddListener(() =>
            {
                _ = LinkEmailToGoogleAccount();
            });
        }
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
        }
        else
        {
            Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
        }


        if (auth.CurrentUser != null)
        {
            _currentUser = auth.CurrentUser;

            if (!currentUser.IsEmailVerified)
            {
                ShowPopup("이메일 인증이 완료되지 않았습니다. 인증 후 다시 로그인해주세요.");
                auth.SignOut();
                return;
            }

            ShowPopup("자동 로그인되었습니다. 계속 진행하려면 확인 버튼을 눌러주세요.", true); // 자동 로그인 후 씬 전환
        }


        _downloadManager = GetComponent<DownloadManager>();
        _downloadManager.Initialize(this);

        googleLoginBtn.gameObject.SetActive(isMobile);
        linkEmailBtn.gameObject.SetActive(isMobile);
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
            _currentUser = registerTask.User;

            await currentUser.SendEmailVerificationAsync();
            ShowPopup("회원가입이 완료되었습니다. 이메일 인증을 진행해주세요.");

            await SaveUserEmailOnSignUp();
        }
        catch (System.Exception ex)
        {
            ShowError("회원가입 중 오류가 발생했습니다. 잠시 후 다시 시도해주세요.", "회원가입 오류: " + ex.Message);
        }

        SetLoading(false);
    }

    public async Task SaveUserEmailOnSignUp()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null) return;

        string uid = user.UserId;
        string email = user.Email;

        var userData = new Dictionary<string, object>
    {
        { "email", email }
    };

        await FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("users")
            .Child(uid)
            .UpdateChildrenAsync(userData);
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
            _currentUser = loginTask.User;
            await _currentUser.ReloadAsync();

            var userSnapshot = await dbRef.Child("users").Child(_currentUser.UserId).GetValueAsync();
            bool isLoggedIn = userSnapshot.Child("isLoggedIn").Value?.ToString() == "true";
            string lastDeviceId = userSnapshot.Child("lastLoginDeviceId").Value?.ToString();

            if (isLoggedIn && lastDeviceId != deviceId)
            {
                ShowPopup("다른 기기에서 이미 로그인되어 있습니다. 로그아웃 후 다시 시도해주세요.");
                auth.SignOut();
                SetLoading(false);
                return;
            }

            if (!_currentUser.IsEmailVerified)
            {
                ShowPopup("이메일 인증이 완료되지 않았습니다. 인증 후 다시 로그인해주세요.");
                resendEmailBtn.gameObject.SetActive(true);
                auth.SignOut();
                SetLoading(false);
                return;
            }

            await dbRef.Child("users").Child(_currentUser.UserId).UpdateChildrenAsync(new Dictionary<string, object>
        {
            { "isLoggedIn", true },
            { "lastLoginDeviceId", deviceId }
        });

            resendEmailBtn.gameObject.SetActive(false);

            // ✅ 다운로드 완료 후 로그인 성공 메시지 표시
            await _downloadManager.DownloadJsonToLocal(fromGameBase: false);

            ShowPopup("로그인 성공", true);
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
            _currentUser = loginTask.User;

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

            _currentUser = linkTask.User;
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
            // 현재 액티비티에서 직접 메소드를 호출하는 대신,
            // 저희가 만든 프록시 액티비티를 실행하는 인텐트(Intent)를 만듭니다.
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // GoogleSignInProxyActivity를 명시적으로 지정하여 인텐트를 생성합니다.
            // 이때, 저희 라이브러리의 네임스페이스가 아닌, 실제 클래스의 패키지 경로를 사용합니다.
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", currentActivity, new AndroidJavaClass("com.Avengers.projectNemesis.GoogleSignInProxyActivity"));
            currentActivity.Call("startActivity", intent);
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
        //SetLoading(true);

        try
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            FirebaseUser firebaseUser = await auth.SignInWithCredentialAsync(credential);

            _currentUser = firebaseUser;
            ShowPopup("Google 로그인 성공", true);

            await _downloadManager.DownloadJsonToLocal(fromGameBase: false);
        }
        catch (System.Exception ex)
        {
            ShowError("Google 로그인 중 오류가 발생했습니다.", "Google 로그인 오류: " + ex.Message);
            auth.SignOut();
        }

        SetLoading(false);
    }
    public void ShowPopup(string message, bool changeSceneOnConfirm = false)
    {
				shouldChangeScene = changeSceneOnConfirm;

				if (popUpMsg != null)
						popUpMsg.text = message;

				if (popUpPanel != null)
						popUpPanel.SetActive(true);

				Debug.Log(message);

				if (popUpConfirmBtn != null)
				{
						popUpConfirmBtn.gameObject.SetActive(true);
						popUpConfirmBtn.onClick.RemoveAllListeners();
						// 팝업 확인 버튼 클릭 시 동작을 비동기(async)로 변경하고 다운로드 로직 추가
						popUpConfirmBtn.onClick.AddListener(async () =>
						{
								if (popUpPanel != null)
										popUpPanel.SetActive(false);

								popUpConfirmBtn.gameObject.SetActive(false);

								if (shouldChangeScene)
								{
										// 로딩 패널을 띄워서 사용자에게 데이터 로드 중임을 알림
										SetLoading(true);

										try
										{
												// DownloadJsonToLocal(false) 호출 시, 사용자 데이터가 없으면 gameBaseJson을 시도합니다.
												// 이 호출이 완료될 때까지 기다립니다.
												await _downloadManager.DownloadJsonToLocal(fromGameBase: false);

												// 다운로드가 성공적으로 완료되면 씬 전환
												SceneManager.LoadScene(1);
												if (mainCanvas != null)
														mainCanvas.SetActive(false);
										}
										catch (System.Exception ex)
										{
												// 다운로드 중 오류가 발생하면 팝업 메시지를 띄우고 처리
												ShowError("데이터 다운로드 중 오류가 발생했습니다. 다시 시도해주세요.", "자동 로그인 후 데이터 다운로드 오류: " + ex.Message);
												auth.SignOut(); // 오류 발생 시 안전하게 로그아웃 처리
																				// 이 경우 씬 전환은 하지 않고 현재 화면에 머무르거나 로그인 화면으로 돌아가게 됩니다.
										}
										finally
										{
												// 로딩 패널 숨기기
												SetLoading(false);
										}
								}
						});
				}

				if (logoutBtn != null)
        {
            if (message.Contains("자동 로그인"))
            {
                logoutBtn.gameObject.SetActive(true);
                logoutBtn.onClick.RemoveAllListeners();
                logoutBtn.onClick.AddListener(async () =>
                {
                    if (_currentUser != null)
                    {
                        await dbRef.Child("users").Child(_currentUser.UserId).Child("isLoggedIn").SetValueAsync(false);
                    }

                    auth.SignOut();
                    _currentUser = null;

                    logoutBtn.gameObject.SetActive(false);
                    if (popUpPanel != null)
                        popUpPanel.SetActive(false);

                    ShowPopup("로그아웃되었습니다. 다시 로그인해주세요.");
                });
            }
            else
            {
                logoutBtn.gameObject.SetActive(false);
            }
        }
    }


    public void ShowError(string userMessage, string logMessage)
    {
        ShowPopup(userMessage);
        Debug.LogError(logMessage);
    }

    public void SetLoading(bool isActive)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(isActive);
    }

    public void OnClickSceneBtn()
    {
        GameManager.Instance.SceneLoadManager.LoadIntroScene();
        EventBus.SetCanGetInput(true);
    }

    public void ServerStart()
    {
        mainCanvas.SetActive(true);
    }

}
