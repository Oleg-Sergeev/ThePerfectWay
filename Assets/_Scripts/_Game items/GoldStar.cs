using UnityEngine;

public class GoldStar : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * 1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.PlayAudio("CoinPickUp");
        
        _UserInterface.sv.levels[_UserInterface.sv.currentLevel-1].stars[2] = 1;
        UI_Adventure.Instance.OnStar(2);

        gameObject.SetActive(false);
    }
}
