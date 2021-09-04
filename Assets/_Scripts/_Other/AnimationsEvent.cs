using UnityEngine;

public class AnimationsEvent : MonoBehaviour
{
    public void OnNotifyShowed()
    {
        Debug.Log("Showed");
        EventAggregator.animationEvents.Publish(0);
    }

    public void OnNotifyHided()
    {
        Debug.Log("Hided");
        EventAggregator.animationEvents.Publish(1);
    }
}
