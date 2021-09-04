using UnityEngine;

public class JumpBoost : MonoBehaviour
{
    [SerializeField]
    private int jumpForce;

    private void Awake()
    {
        EventAggregator.jumpEvents = new EventManager<JumpBoost>();
        EventAggregator.jumpEvents.Subscribe(Jump);
    }

    private void Jump(JumpBoost obj)
    {
        _Player.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        _UserInterface.sv.boost["Jump"].amount--;
        
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Tutorial") BaseUI.Instance.ShowBoost(false);
        else UI_Tutorial.Instance.ShowBoost(false);
    }
}
