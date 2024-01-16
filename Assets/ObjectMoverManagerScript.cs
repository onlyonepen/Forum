using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;

public class ObjectMoverManagerScript : MonoBehaviour
{
    [HideInInspector]
    public GameObject TargetObject;
    public float MaxDistance = 30;
    public Camera ARCamera;
    bool IsInMoveState = false;

    RaycastHit hit;
    Vector3 Point;
    LayerMask NotObject;
    Touch Touch;
    Vector2 TouchPosition;
    quaternion RotationY;
    float RotateSpeedModifier = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        TargetObject = GameObject.FindGameObjectWithTag("TargetObject");
        NotObject =~ TargetObject.layer;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInMoveState == true)
        {
            //MoveObjectLogic
            //Ray ray = ARCamera.ScreenPointToRay(new Vector3(-1/2 ,-1/2 , 0));
            if (Physics.Raycast(ARCamera.ScreenPointToRay(new Vector3(-1/2,-1/2,0))))
            {
                TargetObject.transform.position = hit.point;
                Debug.Log("Placed");
            }
            else if (!Physics.Raycast(ARCamera.ScreenPointToRay(new Vector3(-1 / 2, -1 / 2, 0))))
            {
                TargetObject.transform.position = (Camera.main.transform.eulerAngles.normalized * MaxDistance) + Camera.main.transform.position;
                Debug.Log("Float Place");
            }

            //RotateObjectLogic
            if (Input.touchCount > 0)
            {
                Touch = Input.GetTouch(0);
                if (Touch.phase == TouchPhase.Moved)
                {
                    RotationY = quaternion.Euler(0f, Touch.deltaPosition.x * RotateSpeedModifier, 0f);
                    transform.rotation = RotationY * transform.rotation;
                }
            }
        }
    }

    public void IsMovingSwitch()
    {
        IsInMoveState = !IsInMoveState;
        if (IsInMoveState)
        {
            Debug.Log("Able to move around");
        }
        if (!IsInMoveState)
        {
            Debug.Log("Disable Movement");
        }
    }
}
