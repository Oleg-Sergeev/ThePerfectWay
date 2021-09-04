using UnityEngine;

public class Emeralds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (EventAggregator.emeraldEvents != null) EventAggregator.emeraldEvents.Publish(this);

        gameObject.SetActive(false);
    }

    public void _SetTrue()
    {
        gameObject.SetActive(true);
    }
}
