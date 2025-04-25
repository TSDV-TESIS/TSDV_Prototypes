using UnityEngine;

namespace Events.Scriptables
{
    [CreateAssetMenu(menuName = "Events/AkWwiseEvent and Callback Channel")]
    public class AkWwiseEventWithCallbackChannelSO : TwoValuesEventChannelSO<AK.Wwise.Event, AkCallbackManager.EventCallback>
    {
    }
}
