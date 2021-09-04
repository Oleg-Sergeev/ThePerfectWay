using UnityEngine;
using System.Collections;

public class HideLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(HideLvl());
    }
    
    IEnumerator HideLvl()
    {
        yield return new WaitForSeconds(5 / _Player.Speed * 10);

        EventAggregator.hideEvents.Publish();
    }
}
