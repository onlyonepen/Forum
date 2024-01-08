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
            Point = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            var ray = Physics.Raycast(Camera.main.transform.position,Point,MaxDistance,NotObject);
            TargetObject.transform.position = hit.point;

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
    }
}
