using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoverManagerScript : MonoBehaviour
{
    bool IsInMoveState = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInMoveState = true)
        {
            //MoveObjectLogic

            //RotateObjectLogic
        }
    }

    public void IsMovingSwitch()
    {
        IsInMoveState = !IsInMoveState;
    }
}
