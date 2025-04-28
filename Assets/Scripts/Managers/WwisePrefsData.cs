using System;

namespace Managers
{
    [Serializable]
    class WwisePrefsData
    {
        public AK.Wwise.RTPC rtpc;
        public string prefId;
        public string flagPrefId;
        public bool shouldBeSetted = true;
    }
}