using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (_UserInterface.sv.boost["Jump"].amount > 0)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Tutorial") BaseUI.Instance.ShowBoost(true);
            else
            {
                UI_Tutorial.Instance.ShowBoost(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Tutorial") BaseUI.Instance.ShowBoost(false);
        else
        {
            UI_Tutorial.Instance.ShowBoost(false);
        }
    }
}
