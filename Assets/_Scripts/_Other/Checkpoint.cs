using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Vector3 checkPos;
    public static Quaternion checkRot;
    public static float checkScore;
    public static bool IsMovingX;

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            checkPos = gameObject.transform.position;
            checkScore = _Player.ScoreFloat;
            IsMovingX = _Player.IsMovingX;
            checkRot = _Player.rb.rotation;
        }
    }
}
