using UnityEngine;

public class TutorialCheckPoints : MonoBehaviour
{
    public int numberTip;
    public bool IsMovingX;
    public static Vector3 checkPos;


    private void OnTriggerEnter(Collider col)
    {
        checkPos = gameObject.transform.position;
        TutorualTriggers.numberTip = numberTip;
        UI_Tutorial.IsMovingX = IsMovingX;
    }
}
