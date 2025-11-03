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

		public GameObject popUpPanel;
		public TextMeshProUGUI popUpMsg;
		public GameObject loginPanel;
		public GameObject loadingPanel;
		public Button _nextSceneBtn;
		public Button resendEmailBtn;

		private FirebaseAuth auth;
		private FirebaseUser currentUser;
		private DatabaseReference dbRef;

		public void Initialize()
		{
				auth = FirebaseAuth.DefaultInstance;
				dbRef = FirebaseDatabase.DefaultInstance.RootReference;

				resendEmailBtn.gameObject.SetActive(false);
				_nextSceneBtn.interactable = false;
		}

		public async void OnClickSignUp()
		{
				if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(pwInput.text))
				{
						ShowPopup("아이디와 패스워드를 모두 입력해주세요");
						return;
				}

				loadingPanel.SetActive(true);
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
						ShowPopup("회원가입 실패: " + ex.Message);
				}

				loadingPanel.SetActive(false);
		}

		public async void OnClickLogin()
		{
				if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(pwInput.text))
				{
						ShowPopup("아이디와 패스워드를 모두 입력해주세요");
						return;
				}

				loadingPanel.SetActive(true);
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
								loadingPanel.SetActive(false);
								return;
						}

						ShowPopup("로그인 성공");
						resendEmailBtn.gameObject.SetActive(false);

						await DownloadJsonToLocal(fromGameBase: false);

						_nextSceneBtn.interactable = true;
				}
				catch (System.Exception ex)
				{
						ShowPopup("로그인 실패: " + ex.Message);
				}

				loadingPanel.SetActive(false);
		}

		public async void OnClickResendEmail()
		{
				loadingPanel.SetActive(true);

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
						ShowPopup("재전송 실패: " + ex.Message);
				}

				loadingPanel.SetActive(false);
		}

		public async void UploadPlayerStatFromLocal()
		{
				loadingPanel.SetActive(true);

				if (currentUser == null)
				{
						ShowPopup(" 로그인된 사용자가 없어 업로드할 수 없습니다.");
						loadingPanel.SetActive(false);
						return;
				}

				if (!File.Exists(Constants.FILE_PATH_PLAYERSTAT))
				{
						ShowPopup("업로드할 JSON 파일이 존재하지 않습니다.");
						loadingPanel.SetActive(false);
						return;
				}

				string jsonText = File.ReadAllText(Constants.FILE_PATH_PLAYERSTAT);

				var data = new Dictionary<string, object>
				{
						{ "playerStatJson", jsonText },
						{ "updateTime", ServerValue.Timestamp }
				};

				await dbRef.Child("playerStats").Child(currentUser.UserId).UpdateChildrenAsync(data);

				Debug.Log(" JSON 파일을 성공적으로 업로드했습니다.");
				loadingPanel.SetActive(false);
		}

		public async void DownloadPlayerStatToLocal()
		{
				await DownloadJsonToLocal(fromGameBase: false);
		}

		private async Task DownloadJsonToLocal(bool fromGameBase)
		{
				loadingPanel.SetActive(true);

				string jsonText = null;

				if (fromGameBase)
				{
						var snapshot = await dbRef.Child("gameBaseJson").GetValueAsync();
						if (snapshot != null && snapshot.Exists)
						{
								jsonText = snapshot.GetRawJsonValue();
						}
						else
						{
								ShowPopup(" 초기 게임 데이터가 Firebase에 없습니다.");
						}
				}
				else
				{
						if (currentUser == null)
						{
								ShowPopup(" 로그인된 사용자가 없습니다.");
								loadingPanel.SetActive(false);
								return;
						}

						var snapshot = await dbRef.Child("playerStats").Child(currentUser.UserId).GetValueAsync();
						if (snapshot != null && snapshot.Exists && snapshot.HasChild("playerStatJson"))
						{
								jsonText = snapshot.Child("playerStatJson").Value.ToString();
						}
						else
						{
								ShowPopup(" 사용자 데이터가 Firebase에 없습니다.");
						}
				}

				if (!string.IsNullOrEmpty(jsonText) && jsonText != "null")
				{
						string folderPath = Path.GetDirectoryName(Constants.FILE_PATH_PLAYERSTAT);
						if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
						File.WriteAllText(Constants.FILE_PATH_PLAYERSTAT, jsonText);
						Debug.Log(" JSON 데이터를 성공적으로 저장했습니다.");
				}
				UploadPlayerStatFromLocal();
				loadingPanel.SetActive(false);
		}

		public async void DeleteAccount()
		{
				loadingPanel.SetActive(true);

				if (currentUser == null)
				{
						ShowPopup(" 로그인된 사용자가 없어 계정을 삭제할 수 없습니다.");
						loadingPanel.SetActive(false);
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

						ShowPopup(" 계정이 성공적으로 삭제되었습니다.");
						SceneManager.LoadScene(0);
				}
				catch (System.Exception ex)
				{
						ShowPopup(" 계정 삭제 실패: " + ex.Message);
				}

				loadingPanel.SetActive(false);
		}

		private void ShowPopup(string message)
		{
				popUpMsg.text = message;
				popUpPanel.SetActive(true);
				Debug.Log(message);
		}

		public void OnClickSceneBtn()
		{
				SceneManager.LoadScene(1);
		}
}
