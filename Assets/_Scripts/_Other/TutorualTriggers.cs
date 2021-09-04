using UnityEngine;

public class TutorualTriggers : MonoBehaviour
{
    public static int numberTip;

    private void Start()
    {
        numberTip = 0;
    }

    private void OnTriggerEnter(Collider col)
    {
        switch (gameObject.tag)
        {
            case "Tip":
                UI_Tutorial.Instance.OnTrigger(numberTip);
                numberTip++;
                break;
        }
    }
}
