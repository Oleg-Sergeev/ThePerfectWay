using UnityEngine;

public class WallKey : MonoBehaviour
{
    public static Color _color;


    private void Start()
    {
        _color = Color.red;
        GetComponent<Renderer>().material.color = _color;
    }

    private void Update ()
    {	
        if (Keys.IsCollected)
        {
            _color = Color.green;
            gameObject.GetComponent<Renderer>().material.color = _color;
        }
        else
        {
            _color = Color.red;
            gameObject.GetComponent<Renderer>().material.color = _color;
        }
	}
}
