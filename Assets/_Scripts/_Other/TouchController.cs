using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour, IPointerDownHandler
{
    public static TouchController Instance;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_UserInterface.IsArcade)
        {
            if (_Player.CanTurn)
            {
                _Player.rb.constraints = RigidbodyConstraints.None;
                _Player.rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

                if (_Player.IsMovingX)
                {
                    _Player.IsMovingX = false;

                    _Player.Instance.transform.rotation *= Quaternion.AngleAxis(90, Vector3.up);
                }
                else if (!_Player.IsMovingX)
                {
                    _Player.IsMovingX = true;

                    _Player.Instance.transform.rotation *= Quaternion.AngleAxis(-90, Vector3.up);
                }
            }
        }
    }

}
