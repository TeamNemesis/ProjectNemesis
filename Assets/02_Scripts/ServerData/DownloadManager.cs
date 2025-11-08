using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DownloadManager : MonoBehaviour
{
    private ServerManager _serverManager;

    public void Initialize(ServerManager serverManager)
    {
        _serverManager = serverManager;
    }

    public async Task<bool> TryUploadPlayerStat()
    {
        if (_serverManager.currentUser == null)
        {
            _serverManager.ShowPopup("로그인된 사용자가 없어 업로드할 수 없습니다.");
            return false;
        }

        if (!File.Exists(Constants.FILE_PATH_PLAYERSTAT))
        {
            _serverManager.ShowPopup("업로드할 JSON 파일이 존재하지 않습니다.");
            return false;
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
            return true;
        }
        catch (System.Exception ex)
        {
            _serverManager.ShowError("업로드 중 오류가 발생했습니다.", "Upload 오류: " + ex.Message);
            return false;
        }
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
            await DownloadJsonToLocal(true); // ✅ 초기 데이터 다운로드
                                             // 다운로드 후 다시 시도
            bool success = await TryUploadPlayerStat();
            _serverManager.SetLoading(false);
            return;
            
        }


        // 파일이 이미 존재하면 바로 업로드 시도
        bool result = await TryUploadPlayerStat();
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
                    Debug.Log("초기 게임 데이터를 서버에서 받아왔습니다.");
                }
                else
                {
                    _serverManager.ShowPopup("초기 게임 데이터가 Firebase에 없습니다.");
                    _serverManager.SetLoading(false);
                    return;
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
                    Debug.Log("사용자 데이터를 서버에서 받아왔습니다.");
                }
                else
                {
                    _serverManager.ShowPopup("사용자 데이터가 Firebase에 없습니다.");
                    await DownloadJsonToLocal(fromGameBase: true);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(jsonText) && jsonText != "null")
            {
                string folderPath = Path.GetDirectoryName(Constants.FILE_PATH_PLAYERSTAT);
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                File.WriteAllText(Constants.FILE_PATH_PLAYERSTAT, jsonText);
                Debug.Log("JSON 데이터를 성공적으로 저장했습니다.");
            }

            else
            {
                _serverManager.ShowPopup("서버에서 받은 데이터가 비어 있습니다.");
            }
        }
        catch (System.Exception ex)
        {
            _serverManager.ShowError("데이터 다운로드 중 오류가 발생했습니다.", "DownloadJson 오류: " + ex.Message);
        }

        _serverManager.SetLoading(false);
    }


    public async Task UploadClearTime(float clearTime)
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null) return;

        string uid = user.UserId;

        var data = new Dictionary<string, object>
    {
        { "time", clearTime },
        { "savedAt", ServerValue.Timestamp } // 서버 시간 기준 저장
    };

        await FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("users")
            .Child(uid)
            .Child("clearTimes")
            .Push()
            .SetValueAsync(data);
    }

    public async Task<List<(string email, float bestTime, long savedAt)>> GetClearTimeRanking()
    {
        var snapshot = await FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("users")
            .GetValueAsync();

        var rankingList = new List<(string email, float bestTime, long savedAt)>();

        foreach (var userNode in snapshot.Children)
        {
            string email = userNode.Child("email").Value?.ToString() ?? "Unknown";
            var clearTimesNode = userNode.Child("clearTimes");

            float bestTime = float.MaxValue;
            long bestSavedAt = 0;

            foreach (var timeEntry in clearTimesNode.Children)
            {
                var timeValue = timeEntry.Child("time").Value?.ToString();
                var savedAtValue = timeEntry.Child("savedAt").Value;

                if (float.TryParse(timeValue, out float t) && savedAtValue != null && long.TryParse(savedAtValue.ToString(), out long timestamp))
                {
                    if (t < bestTime)
                    {
                        bestTime = t;
                        bestSavedAt = timestamp;
                    }
                }
            }

            if (bestTime != float.MaxValue)
            {
                rankingList.Add((email, bestTime, bestSavedAt));
            }
        }

        return rankingList.OrderBy(r => r.bestTime).ToList();
    }
}