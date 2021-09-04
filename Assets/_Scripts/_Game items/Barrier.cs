using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnMouseDown()
    {
        _UserInterface.sv.coin++;
        gameObject.SetActive(false);

        Invoke("SetTrue", 50 / _Player.Speed * 10);
    }

    private void SetTrue()
    {
        gameObject.SetActive(true);
    }
}
