using UnityEngine;
using UnityEngine.UI;

public class CurrentColorScript : MonoBehaviour
{

    public bool getStartingColorFromMaterial;
    public FlexibleColorPicker fcp;
    public Image material;

    private void Start()
    {
        if (getStartingColorFromMaterial)
            fcp.color = material.color;

        fcp.onColorChange.AddListener(OnChangeColor);
    }

    private void OnChangeColor(Color co)
    {
        material.color = co;
    }
}
