using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SceneryManager sceneryManager;

        [Header("Prefs")] [SerializeField] private List<WwisePrefsData> prefs;

        private float defaultRtpcValue = 50f;

        private void Awake()
        {
            sceneryManager.InitScenes();
        }

        private void Start()
        {
            foreach (var wwisePrefsData in prefs)
            {
                CheckRtpc(wwisePrefsData.prefId, wwisePrefsData.flagPrefId, wwisePrefsData.rtpc,
                    wwisePrefsData.shouldBeSetted);
            }
        }

        private void CheckRtpc(string valuePref, string flagPref, AK.Wwise.RTPC rtpc, bool shouldBeSetted)
        {
            bool hasBeenModified = PlayerPrefs.GetInt(flagPref) == 1;
            float value = PlayerPrefs.GetFloat(valuePref);

            Debug.Log(hasBeenModified ? value : defaultRtpcValue);

            if (shouldBeSetted)
                AkSoundEngine.SetRTPCValue(rtpc.Name, hasBeenModified ? value : defaultRtpcValue);
            else if (hasBeenModified)
            {
                AkSoundEngine.SetRTPCValue(rtpc.Name, value);
            }

            ;
        }
    }
}