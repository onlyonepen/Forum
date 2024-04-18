using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogManager : MonoBehaviour
{
    public GameObject Set1;
    public GameObject Set2;
    public GameObject Set3;
    public GameObject Set4;
    public int CurrentSet = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextSet()
    {
        SetActiveSet();
        CurrentSet++;
        if (CurrentSet > 4)
        {
            CurrentSet = 1;
        }
    }
    public void PreviousSet()
    {
        SetActiveSet();
        CurrentSet--;
        if (CurrentSet < 1)
        {
            CurrentSet = 4;
        }
    }

    public void SetActiveSet()
    {
        if (CurrentSet == 1)
        {
            Set1.SetActive(true);
            Set2.SetActive(false);
            Set3.SetActive(false);
            Set4.SetActive(false);
        }
        if (CurrentSet == 2)
        {
            Set1.SetActive(false);
            Set2.SetActive(true);
            Set3.SetActive(false);
            Set4.SetActive(false);
        }
        if (CurrentSet == 3)
        {
            Set1.SetActive(false);
            Set2.SetActive(false);
            Set3.SetActive(true);
            Set4.SetActive(false);
        }
        if (CurrentSet == 4)
        {
            Set1.SetActive(false);
            Set2.SetActive(false);
            Set3.SetActive(false);
            Set4.SetActive(true);
        }
    }
}
