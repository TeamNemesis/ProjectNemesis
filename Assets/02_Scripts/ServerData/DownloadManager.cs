using Firebase.Database;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DownloadManager : MonoBehaviour
{
    private ServerManager _serverManager;

    public void Initialize(ServerManager serverManager)
    {
        _serverManager = serverManager;
    }

    public async void UploadPlayerStatFromLocal()
    {
        _serverManager.SetLoading(true);

        if (_serverManager.currentUser == null)
        {
            _serverManager.ShowPopup("로그인된 사용자가 없어 업로드할 수 없습니다.");
            _serverManager.SetLoading(false);
            return;
        }

        if (!File.Exists(Constants.FILE_PATH_PLAYERSTAT))
        {
            _serverManager.ShowPopup("업로드할 JSON 파일이 존재하지 않습니다.");
            await DownloadJsonToLocal(true); // ✅ await 추가
            return;
        }

        string jsonText = File.ReadAllText(Constants.FILE_PATH_PLAYERSTAT);

        var data = new Dictionary<string, object>
    {
        { "playerStatJson", jsonText },
        { "updateTime", ServerValue.Timestamp }
    };

        try
        {
            await _serverManager.dbRef.Child("playerStats").Child(_serverManager.currentUser.UserId).UpdateChildrenAsync(data);
            Debug.Log("JSON 파일을 성공적으로 업로드했습니다.");
        }
        catch (System.Exception ex)
        {
            _serverManager.ShowError("업로드 중 오류가 발생했습니다.", "Upload 오류: " + ex.Message);
        }

        _serverManager.SetLoading(false);
    }



    public async void DownloadPlayerStatToLocal()
    {
        await DownloadJsonToLocal(fromGameBase: false);
    }

    public async Task DownloadJsonToLocal(bool fromGameBase)
    {
        _serverManager.SetLoading(true);

        string jsonText = null;

        try
        {
            if (fromGameBase)
            {
                var snapshot = await _serverManager.dbRef.Child("gameBaseJson").GetValueAsync();
                if (snapshot != null && snapshot.Exists)
                {
                    jsonText = snapshot.GetRawJsonValue();
                }
                else
                {
                    _serverManager.ShowPopup("초기 게임 데이터가 Firebase에 없습니다.");
                }
            }
            else
            {
                if (_serverManager.currentUser == null)
                {
                    _serverManager.ShowPopup("로그인된 사용자가 없습니다.");
                    _serverManager.SetLoading(false);
                    return;

                }

                var snapshot = await _serverManager.dbRef.Child("playerStats").Child(_serverManager.currentUser.UserId).GetValueAsync();
                if (snapshot != null && snapshot.Exists && snapshot.HasChild("playerStatJson"))
                {
                    jsonText = snapshot.Child("playerStatJson").Value.ToString();
                }
                else
                {
                    _serverManager.ShowPopup("사용자 데이터가 Firebase에 없습니다.");
                }
            }

            if (!string.IsNullOrEmpty(jsonText) && jsonText != "null")
            {
                string folderPath = Path.GetDirectoryName(Constants.FILE_PATH_PLAYERSTAT);
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                File.WriteAllText(Constants.FILE_PATH_PLAYERSTAT, jsonText);
                Debug.Log("JSON 데이터를 성공적으로 저장했습니다.");
            }

          
        }
        catch (System.Exception ex)
        {
            _serverManager.ShowError("데이터 다운로드 중 오류가 발생했습니다.", "DownloadJson 오류: " + ex.Message);
        }

        _serverManager.SetLoading(false);
    }
}
