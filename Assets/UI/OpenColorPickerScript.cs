using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenColorPickerScript : MonoBehaviour
{
    public GameObject ColorPicker;
    public GameObject Painter;
    public bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenColorPicker()
    {
        Debug.Log("Open color picker");
        isOpen = true;
        ColorPicker.SetActive(true);
        Painter.SetActive(false);
    }

    public void CloseColorpicker()
    {
        Debug.Log("Close color picker");
        isOpen = false;
        ColorPicker.SetActive(false);
        Painter.SetActive(true);
    }
}