using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerProbably : MonoBehaviour
{
    public GameObject Panel;
    public float PullPanelSpeed = 50;
    public float MaxPullDistance = 1000;
    public GameObject MoverButton;
    public GameObject PainterManager;
    bool isup = false;
    Vector3 StartPanelPosition;

    // Start is called before the first frame update
    void Start()
    {
        StartPanelPosition = Panel.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isup)
        {
            if (Panel.transform.position.y <= StartPanelPosition.y + MaxPullDistance)
            {
                Panel.transform.position += Vector3.up * PullPanelSpeed;
            }
        }
        else
        {
            if (Panel.transform.position.y >= StartPanelPosition.y)
            {
                Panel.transform.position += -Vector3.up * PullPanelSpeed;
            }
        }
    }

    public void pullpanel()
    {
        isup = !isup;
        MoverButton.GetComponent<Button>().enabled = !isup;
        PainterManager.SetActive(!isup);
    }
}
