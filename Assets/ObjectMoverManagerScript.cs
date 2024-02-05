using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMoverManagerScript : MonoBehaviour
{
    [HideInInspector]
    public GameObject TargetObject;
    public float MaxDistance = 2;
    public Camera ARCamera;
    bool IsInMoveState = false;
    public GameObject PainterManager;
    public GameObject PanelPullButton;

    RaycastHit hit;
    Vector3 Point;
    LayerMask NotObject;
    Touch Touch;
    Vector2 TouchPosition;
    quaternion RotationY;
    float RotateSpeedModifier = 0.01f;
    GameObject EmptyObject;

    // Start is called before the first frame update
    void Start()
    {
        TargetObject = GameObject.FindGameObjectWithTag("TargetObject");
        NotObject =~ TargetObject.layer;
        EmptyObject = new GameObject("MovePoint");
        EmptyObject.transform.position = gameObject.transform.position + Vector3.forward * MaxDistance;
        EmptyObject.transform.parent = ARCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInMoveState == true)
        {
            //MoveObjectLogic
            //Ray ray = ARCamera.ScreenPointToRay(new Vector3(-1 / 2, -1 / 2, 0));
            //if (Physics.Raycast(ARCamera.ScreenPointToRay(new Vector3(-1 / 2, -1 / 2, 0))))
            //{
            //    TargetObject.transform.position = hit.point;
            //    Debug.Log("Placed");
            //}
            //else if (!Physics.Raycast(ARCamera.ScreenPointToRay(new Vector3(-1 / 2, -1 / 2, 0))))
            //{
            //    TargetObject.transform.position = (ARCamera.transform.eulerAngles.normalized * MaxDistance) + ARCamera.transform.position;
            //    Debug.Log("Float Place");
            //}
            //Debug.Log(TargetObject.transform.position);
            TargetObject.transform.position = EmptyObject.transform.position;

            //RotateObjectLogic
            if (Input.touchCount > 0)
            {
                Touch = Input.GetTouch(0);
                Debug.Log("Touch");
                if (Touch.phase == TouchPhase.Moved)
                {
                    RotationY = quaternion.Euler(0f, -Touch.deltaPosition.x * RotateSpeedModifier, 0f);
                    TargetObject.transform.rotation = RotationY * TargetObject.transform.rotation;
                    Debug.Log("Rotate " +  RotationY);
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
            PainterManager.SetActive(false);
            PanelPullButton.GetComponent<Button>().enabled = false;
        }
        if (!IsInMoveState)
        {
            TargetObject.transform.parent = null;
            Debug.Log("Disable Movement");
            PainterManager.SetActive(true);
            PanelPullButton.GetComponent<Button>().enabled = true;
        }
    }
}
