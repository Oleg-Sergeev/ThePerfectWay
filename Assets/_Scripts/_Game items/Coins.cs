using UnityEngine;

public class Coins : MonoBehaviour
{
    public int coin;


    private void OnTriggerEnter(Collider other)
    {
        if(EventAggregator.coinEvents != null) EventAggregator.coinEvents.Publish(this);

        gameObject.SetActive(false);
    }

    public void _SetTrue()
    {
        gameObject.SetActive(true);
    }
}
