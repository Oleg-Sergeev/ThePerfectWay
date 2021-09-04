using UnityEngine;

public class RotateY : MonoBehaviour
{
	void FixedUpdate ()
    {
        transform.Rotate(Vector3.up);
	}
}
