using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventAggregator.loadEvents.Publish();
    }
}
