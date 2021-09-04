using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private bool IsMovingX;
    public static bool IsEnabled;
    public Transform _camera, player;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            if (eventData.delta.x > 0)
            {
                if (Mathf.Abs(90 - player.localEulerAngles.y) > 0.01f) player.transform.rotation *= Quaternion.AngleAxis(90, Vector3.up);
            }
            else
            {
                if (Mathf.Abs(270 - player.localEulerAngles.y) > 0.01f) player.transform.rotation *= Quaternion.AngleAxis(-90, Vector3.up);
            }
        }
    }

    public void OnDrag(PointerEventData eventData){}
}
