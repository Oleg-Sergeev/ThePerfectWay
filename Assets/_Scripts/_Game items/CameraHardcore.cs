using UnityEngine;

public class CameraHardcore : MonoBehaviour
{
    private int minRotationSpeed = 3, coefficient = 25;
    private Vector3 _offset;
    public Transform _target;


    private void Start()
    {
        _offset = _target.position - transform.position;
    }

    private void LateUpdate()
    {
        transform.position = _target.position - _offset;

        transform.rotation = Quaternion.Lerp(transform.rotation, _target.rotation, (minRotationSpeed + (_Player.Speed / coefficient)) * Time.deltaTime);
    }
}