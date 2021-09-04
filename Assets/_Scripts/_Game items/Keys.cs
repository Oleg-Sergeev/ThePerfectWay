using UnityEngine;

public class Keys : MonoBehaviour
{
    public static bool IsCollected;


    private void Start()
    {
        IsCollected = false;
    }

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up * 2);
    }

    public void OnTriggerEnter(Collider other)
    {
        IsCollected = true;

        if (other.gameObject.name == "Player") AudioManager.PlayAudio("KeyPickUp");

        gameObject.SetActive(false);
        Invoke("_SetTrue", 5);
    }

    private void _SetTrue()
    {
        gameObject.SetActive(true);
    }
}
