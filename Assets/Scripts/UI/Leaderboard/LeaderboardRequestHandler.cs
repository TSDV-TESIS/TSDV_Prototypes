using System;
using System.Collections.Generic;
using Leaderboard;
using Player;
using UnityEngine;

namespace UI.Leaderboard
{
    public class LeaderboardRequestHandler : MonoBehaviour
    {
        [SerializeField] private GameObject leaderboardTable;
        [SerializeField] private GameObject loadingObject;
        [SerializeField] private GameObject errorObject;
        [SerializeField] private GameObject scrollView;
        [SerializeField] private LeaderboardData leaderboardData;
        [SerializeField] private LeaderboardCreateEvent createNewTimeEvent;
        [SerializeField] private PlayerName playerName;
        [SerializeField] private LevelEnum level;

        [SerializeField] private List<LeaderboardRowSetter> pageRows;

        void OnEnable()
        {
            leaderboardData.requestObtained.AddListener(HandleLeaderboard);
            createNewTimeEvent.createNewTimeFinish.AddListener(HandleGetLeaderboard);
        }

        private void OnDisable()
        {
            leaderboardData.requestObtained.RemoveListener(HandleLeaderboard);
            createNewTimeEvent.createNewTimeFinish.RemoveListener(HandleGetLeaderboard);
        }

        private void HandleGetLeaderboard()
        {
            if (createNewTimeEvent.hasError)
            {
                Debug.LogError("Error creating new time.");
                loadingObject.SetActive(false);
                errorObject.SetActive(true);
                return;
            }

            leaderboardData.requestData.Invoke(level);
        }

        public void HandleSetTime(int time)
        {
            loadingObject.SetActive(true);
            scrollView.SetActive(false);
            createNewTimeEvent.createNewTime.Invoke(new LeaderboardRow(playerName.playerName, time, level));
        }

        private void HandleLeaderboard()
        {
            Debug.Log("OBTAINED!");

            if (leaderboardData.hasError)
            {
                Debug.LogError("ERROR obtaining leaderboard!");
                loadingObject.SetActive(false);
                errorObject.SetActive(true);
                return;
            }

            loadingObject.SetActive(false);
            scrollView.SetActive(true);

            for (int i = 0; i < leaderboardData.data.Length && i < pageRows.Count; i++)
            {
                Debug.Log($"{leaderboardData.data[i].name} ASD");
                pageRows[i].SetData(i + 1, leaderboardData.data[i].name, leaderboardData.data[i].timeBeaten);
            }
        }
    }
}