using UnityEngine;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    private bool IsSetFalse, IsContinue;
    private float speedAlpha;
    private Image image_;
    private Color color_;


    private void Start()
    {
        speedAlpha = 1;
        image_ = GetComponent<Image>();
        color_ = new Color(1, 1, 1, Random.Range(0f,1f));
        IsSetFalse = false;
        IsContinue = true;
    }

    void FixedUpdate ()
    {
        gameObject.transform.Rotate(- Vector3.forward * 0.5f);

        if (image_.color.a > 0 && IsSetFalse == false)
        {
            color_.a -= speedAlpha * Time.deltaTime;
            image_.color = new Color(1, 1, 1, color_.a);
        }
        else if (IsContinue && IsSetFalse == false)
        {

            Invoke("SetActive", Random.Range(0.5f, 1.5f));

            IsContinue = false;
        }

        if (image_.color.a < 1 && IsSetFalse)
        {
            color_.a += speedAlpha * Time.deltaTime;
            image_.color = new Color(1, 1, 1, color_.a);
        }
        else if (IsContinue && IsSetFalse)
        {
            Invoke("SetActive", Random.Range(0.5f, 1.5f));

            IsContinue = false;
        }
    }

    void SetActive()
    {
        IsSetFalse = !IsSetFalse;

        speedAlpha = Random.Range(1.5f, 2.5f);

        IsContinue = true;
    }
}
