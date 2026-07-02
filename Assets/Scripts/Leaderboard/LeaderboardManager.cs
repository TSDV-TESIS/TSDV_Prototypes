using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Leaderboard
{
    public class LeaderboardManager : MonoBehaviour
    {
        private static readonly string LeaderboardURL = "https://crimson-leaderboard.onrender.com";

        [SerializeField] private LeaderboardData leaderboardData;
        [SerializeField] private LeaderboardCreateEvent leaderboardCreateEvent;

        public void OnEnable()
        {
            leaderboardData.requestData.AddListener(RequestData);
            leaderboardCreateEvent.createNewTime.AddListener(HandleNewTime);
        }

        public void OnDisable()
        {
            leaderboardData.requestData.RemoveListener(RequestData);
            leaderboardCreateEvent.createNewTime.RemoveListener(HandleNewTime);
        }

#if UNITY_EDITOR
        [ContextMenu("ClearPlayers")]
        private void ClearPlayers()
        {
            StartCoroutine(HandleClearPlayersRequest());
        }
#endif
        private void HandleNewTime(LeaderboardRow row)
        {
            StartCoroutine(HandleNewRow(row));
        }

        private IEnumerator HandleNewRow(LeaderboardRow row)
        {
            UnityWebRequest request = new UnityWebRequest(LeaderboardURL + "/leaderboard", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(row));
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            Debug.Log("Create new row Status Code: " + request.responseCode);

            leaderboardCreateEvent.hasError = request.responseCode != 201;
            leaderboardCreateEvent.createNewTimeFinish.Invoke();
        }

        private IEnumerator HandleClearPlayersRequest()
        {
            UnityWebRequest request = new UnityWebRequest(LeaderboardURL + "/clear", "POST");
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            Debug.Log($"Cleared {request.downloadHandler.text} players, Status Code: " + request.responseCode);
        }

        public void RequestData(LevelEnum level)
        {
            leaderboardData.isLoading = true;
            StartCoroutine(RequestLeaderboard(LeaderboardRow.GetLevelNameByLevelEnum(level)));
        }

        private IEnumerator RequestLeaderboard(string levelName)
        {
            UnityWebRequest getLeaderboardRequest = UnityWebRequest.Get(LeaderboardURL + "/leaderboard?type=" + levelName);
            yield return getLeaderboardRequest.SendWebRequest();

            switch (getLeaderboardRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("There was an error obtaining the leaderboard data.");
                    leaderboardData.hasError = true;
                    leaderboardData.isLoading = false;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("There was an HTTP error obtaining the leaderboard data.");
                    leaderboardData.hasError = true;
                    leaderboardData.isLoading = false;
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log($"Received: " + getLeaderboardRequest.downloadHandler.text);
                    GetLeaderboardResponse leaderboardResponse =
                        JsonUtility.FromJson<GetLeaderboardResponse>(getLeaderboardRequest.downloadHandler.text);

                    leaderboardData.data = leaderboardResponse.data;
                    leaderboardData.leaderboardLevel = levelName;
                    leaderboardData.isLoading = false;
                    leaderboardData.hasError = false;

                    break;
            }

            leaderboardData.requestObtained.Invoke();
        }
    }
}