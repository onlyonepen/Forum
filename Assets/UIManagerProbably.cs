using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerProbably : MonoBehaviour
{
    public GameObject Panel;
    public GameObject MoverButton;
    public GameObject PainterManager;
    bool isup = false;
    Vector3 StartPanelPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pullpanel()
    {
        isup = !isup;
        Panel.SetActive(isup);
    }
}
