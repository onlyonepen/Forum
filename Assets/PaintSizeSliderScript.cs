using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintSizeSliderScript : MonoBehaviour
{
    public GameObject PainterManager;
    int FlooredSliderValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FlooredSliderValue = Mathf.RoundToInt(gameObject.GetComponent<Slider>().value);
        PainterManager.GetComponent<painterScript>().BrushSize = FlooredSliderValue;
    }
}
