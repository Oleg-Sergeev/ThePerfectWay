using UnityEngine;

public class _Camera : MonoBehaviour
{
    private Vector3 _offset;
    public Transform _target;


    private void Awake()
    {
        _offset = _target.position - transform.position;
    }

    private void LateUpdate()
    {
        transform.position = _target.position - _offset;
    }
}
