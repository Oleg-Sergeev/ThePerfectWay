using UnityEngine;

public class Barrier_MainMenu : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
