using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ColorImage : MonoBehaviour
{
    public ColorPicker picker;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();

        if (_UserInterface.GetDictionaryValue(_UserInterface.Instance.texts, "Trail").gameObject.activeSelf) image.color = _UserInterface.sv.colorTrail;
        else image.color = _UserInterface.sv.colorFigure;

        picker.onValueChanged.AddListener(ColorChanged);
    }

    private void OnDestroy()
    {
        picker.onValueChanged.RemoveListener(ColorChanged);
    }

    private void ColorChanged(Color newColor)
    {
        image.color = newColor;
    }
}
